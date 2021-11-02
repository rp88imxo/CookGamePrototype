using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Models;
using CookingPrototype.Services;
using CookingPrototype.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CookingPrototype.Kitchen.Controllers {

public class CustomersConfig {
	public int CustomersTargetNumber { get; set; } = 15;
	public float CustomerWaitTime { get; set; }= 18f;
	public float CustomerSpawnTime{ get; set; } = 3f;
	public int MaxOrdersCount { get; set; } = 3;
}

public class QueueCustomer {
	public CustomerModel Customer { get; }
	private Timer WaitTimer { get; set; }

	public float TimeLeft => WaitTimer != null
		? (float)WaitTimer.TimeLeft.TotalSeconds
		: 0f;

	public bool HasAnyOrders => Customer.Orders.Count > 0;
	
	private Action<CustomerModel> _onTimerStarted;
	private Action<CustomerModel, TimeSpan> _onTimerTicked;
	private Action<CustomerModel> _onTimerCompleted;
	
	
	public QueueCustomer(
		CustomerModel customer,TimeSpan initialTime,
		Action<CustomerModel> onTimerStarted,
		Action<CustomerModel, TimeSpan> onTimerTicked,
		Action<CustomerModel> onTimerCompleted
		) {
		_onTimerStarted = onTimerStarted;
		_onTimerTicked = onTimerTicked;
		_onTimerCompleted = onTimerCompleted;

		Customer = customer;
		
		WaitTimer = new Timer(initialTime, (1/60f).ToTimeSpanSeconds())
				.OnStarted(OnTimerStarted)
				.OnTick(OnTimerTicked)
				.OnCompleted(OnTimerCompleted)
				.SetTickCallbackOnStarted()
				.Start();
	}

	#region TIMER_CALLBACKS_WRAPPERS

	private void OnTimerCompleted() {
		_onTimerCompleted?.Invoke(Customer);
	}

	private void OnTimerTicked(TimeSpan obj) {
		_onTimerTicked?.Invoke(Customer, obj);
	}

	private void OnTimerStarted() {
		_onTimerStarted?.Invoke(Customer);
	}

	#endregion
	
	public void StopTimer()
	{
		WaitTimer?.Stop();
		WaitTimer = null;
	}

	public void PauseTimer()
	{
		WaitTimer?.Stop();
	}

	public void ResumeTimer()
	{
		WaitTimer?.Resume();
	}
}

	public class CustomersControllerNew {

		public const int TOTAL_CUSTOMERS_ICONS = 4;
		
		// Should be injected via DI
		private readonly GameplayMainScreenView _gameplayMainScreenView;
		private readonly CustomersViewPresenter _customersViewPresenter;
		
		private readonly OrderGeneratorService _orderGeneratorService;
		
		private CustomersConfig _currentCustomersConfig;
		
		private Timer _customersTimerGenerator;

		private int _totalActiveCustomers;
		private int _lastCustomerId;

		private readonly Dictionary<int, QueueCustomer> _queueCustomers;
		
		public CustomersControllerNew(OrderGeneratorService orderGeneratorService,GameplayMainScreenView gameplayMainScreenView) {
			_gameplayMainScreenView = gameplayMainScreenView;
			_customersViewPresenter = _gameplayMainScreenView.CustomersViewPresenter;
			_orderGeneratorService = orderGeneratorService;
			_queueCustomers = new Dictionary<int, QueueCustomer>();
		}

		public void InitGameSession(CustomersConfig config) {
			_lastCustomerId = 0;
			_totalActiveCustomers = 0;
			
			_currentCustomersConfig = config;
			_customersViewPresenter.Show();
			
			_customersTimerGenerator?.Stop();
			_customersTimerGenerator = new Timer(
				_currentCustomersConfig.CustomerSpawnTime
					.ToTimeSpanSeconds(),
				1f.ToTimeSpanSeconds())
				.OnCompleted(TryGenerateCustomer)
				.Start();
		}

		private void TryGenerateCustomer() {
			_customersTimerGenerator.Reset();
			if ( _customersViewPresenter.HasFreeSpawnPoint) {
				var customerModel = GenerateCustomer();
				
				var queueCustomer = new QueueCustomer(customerModel,
					_currentCustomersConfig.CustomerWaitTime
						.ToTimeSpanSeconds(),
					null,
					ONTimerTicked,
					ONTimerCompleted);
				
				_customersViewPresenter.AddCustomerView(new CustomerViewModel {
					Id = customerModel.Id,
					CustomerIconName = customerModel.IconPath,
					OrderInitialTime = _currentCustomersConfig.CustomerWaitTime,
					OrdersViewsNames = _orderGeneratorService.GenerateRandomOrder().Foods.Select(x=> $"{x.Name}").ToList()
				});
				
				_queueCustomers.Add(customerModel.Id, queueCustomer);
			}

			_customersTimerGenerator.Start();
		}
		
		#region QUEUE_CUSTOMERS_TIMER_CALLBACKS

		private void ONTimerCompleted(CustomerModel obj) {
			_queueCustomers.Remove(obj.Id);
			_customersViewPresenter.RemoveCustomerViewModelById(obj.Id);
		}

		private void ONTimerTicked(CustomerModel arg1, TimeSpan arg2) {
			_customersViewPresenter.RepaintCustomerViewModelTimerById(
				arg1.Id,
				arg2);
		}

		#endregion

		#region SOME_PUBLIC_API
		
		/// <summary>
		///  Пытаемся обслужить посетителя с заданным заказом и наименьшим оставшимся временем ожидания.
		///  Если у посетителя это последний оставшийся заказ из списка, то отпускаем его.
		/// </summary>
		/// <param name="orderModel">Заказ, который пытаемся отдать</param>
		/// <returns>Флаг - результат, удалось ли успешно отдать заказ</returns>
		public bool ServeOrder(OrderModel orderModel) {
			var queueCustomer = _queueCustomers.Values
				.Where(x
					=> x.Customer.Orders.Any(order
						=> order.Equals(orderModel)))
				.OrderBy(x=> x.TimeLeft)
				.FirstOrDefault();

			if ( queueCustomer == null ) {
				return false;
			}

			_customersViewPresenter.ServeOrderByName(
				queueCustomer.Customer.Id,
				orderModel.Name);

			queueCustomer.Customer.Orders.Remove(orderModel);

			if ( !queueCustomer.HasAnyOrders ) {
				_queueCustomers.Remove(queueCustomer.Customer.Id);
				_customersViewPresenter.RemoveCustomerViewModelById(queueCustomer.Customer.Id);
			}
			
			return true;
		}

		#endregion
		

		// Should be moved outside so we can fetch data from server or internal generator and act like provider via some interface type like ICustomerModelProvider
		private CustomerModel GenerateCustomer() {
			
			var customer = new CustomerModel() 
			{
				Id = _lastCustomerId++,
				IconPath = $"Images/Customers/char_{Random.Range(0,  TOTAL_CUSTOMERS_ICONS)}",
				Orders = null
			};

			return customer;
		}
	}
}

