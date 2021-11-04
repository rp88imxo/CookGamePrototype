using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.Controllers;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Models;
using CookingPrototype.Services;
using CookingPrototype.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CookingPrototype.Kitchen.Controllers {
public class CustomersConfig {
	public int TotalCustomersNumber { get; set; } = 15;
	public float CustomerWaitTime { get; set; } = 18f;
	public float CustomerSpawnTime { get; set; } = 3f;
	public int MaxOrdersCount { get; set; } = 3;
	public List<OrderModel> LevelOrders { get; set; }
	public int NeededFoodsInOrderToBeServed => LevelOrders?.Count ?? -1;
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


	public QueueCustomer(CustomerModel customer,
		TimeSpan initialTime,
		Action<CustomerModel> onTimerStarted,
		Action<CustomerModel, TimeSpan> onTimerTicked,
		Action<CustomerModel> onTimerCompleted) {
		_onTimerStarted = onTimerStarted;
		_onTimerTicked = onTimerTicked;
		_onTimerCompleted = onTimerCompleted;

		Customer = customer;

		WaitTimer = new Timer(initialTime, (1 / 60f).ToTimeSpanSeconds())
			.OnStarted(OnTimerStarted)
			.OnTick(OnTimerTicked)
			.OnCompleted(OnTimerCompleted)
			.SetTickCallbackOnStarted();
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

	public void StartTimer() {
		WaitTimer?.Start();
	}
	
	public void StopTimer() {
		WaitTimer?.Stop();
		WaitTimer = null;
	}

	public void PauseTimer() {
		WaitTimer?.Stop();
	}

	public void ResumeTimer() {
		WaitTimer?.Resume();
	}
}

public class CustomersOrdersSessionModelData {
	public int TotalServedCustomers { get; set; }
	public int TotalGeneratedCustomers { get; set; }
	public int TotalServedFoodsInOrder { get; set; }
	public int LastCustomerId { get; set; }

	public void Reset() {
		TotalServedCustomers = default;
		TotalGeneratedCustomers = default;
		TotalServedFoodsInOrder = default;
		LastCustomerId = default;
	}
}

public class CustomersControllerNew {
	public const int TOTAL_CUSTOMERS_ICONS = 4;

	/// <summary>
	/// Customer fully served
	/// </summary>
	public static event Action<int,int> CustomerServed;

	/// <summary>
	/// Served some order(part of the full order) of customer
	/// </summary>
	public static event Action<OrderModel> CustomerOrderServed;

	/// <summary>
	/// Called when we hit some condition to end the session
	/// </summary>
	public static event Action<bool> CustomerTaskCompleted;

	public int TotalServedOrders
		=> _sessionModel?.TotalServedFoodsInOrder ?? 0;

	public int TotalTargetOrders => _currentCustomersConfig?.NeededFoodsInOrderToBeServed ?? 0;

	// Should be injected via DI
	private readonly GameplayMainScreenView _gameplayMainScreenView;
	private readonly CustomersViewPresenter _customersViewPresenter;

	private readonly OrderGeneratorService _orderGeneratorService;

	private CustomersOrdersSessionModelData
		_sessionModel;

	private CustomersConfig _currentCustomersConfig;

	private Timer _customersTimerGenerator;

	private readonly Dictionary<int, QueueCustomer> _queueCustomers;

	public CustomersControllerNew(
		OrderGeneratorService orderGeneratorService,
		GameplayMainScreenView gameplayMainScreenView) {
		_gameplayMainScreenView = gameplayMainScreenView;
		_customersViewPresenter =
			_gameplayMainScreenView.CustomersViewPresenter;
		_orderGeneratorService = orderGeneratorService;
		_queueCustomers = new Dictionary<int, QueueCustomer>();

		_sessionModel = new CustomersOrdersSessionModelData();
	}
	

	public void InitGameSession(CustomersConfig config) {
		GameplayControllerNew.SessionEnded += HandleSessionEnded;

		_sessionModel.Reset();

		_currentCustomersConfig = config;
		_customersViewPresenter.Show();

		_customersTimerGenerator?.Stop();
		_customersTimerGenerator = new Timer(_currentCustomersConfig
					.CustomerSpawnTime
					.ToTimeSpanSeconds(),
				1f.ToTimeSpanSeconds())
			.OnCompleted(TryGenerateCustomer)
			.Start();
	}

	private void HandleSessionEnded() {
		GameplayControllerNew.SessionEnded -= HandleSessionEnded;
	}
	
	private void TryGenerateCustomer() {
		_customersTimerGenerator.Reset();

		if ( _customersViewPresenter.HasFreeSpawnPoint ) {
			if ( !(_sessionModel
					.TotalGeneratedCustomers
				< _currentCustomersConfig.TotalCustomersNumber) ) {
				CustomerTaskCompleted?.Invoke(
					CheckTaskCompletion(_sessionModel));
				return;
			}

			var customerModel = GenerateCustomer();

			var queueCustomer = new QueueCustomer(customerModel,
				_currentCustomersConfig.CustomerWaitTime
					.ToTimeSpanSeconds(),
				null,
				ONTimerTicked,
				ONTimerCompleted);

			_customersViewPresenter.AddCustomerView(
				new CustomerViewModel {
					Id = customerModel.Id,
					CustomerIconName = customerModel.IconPath,
					OrderInitialTime =
						_currentCustomersConfig.CustomerWaitTime,
					OrdersViewsNames = _orderGeneratorService
						.GenerateRandomOrder()
						.Foods.Select(x => $"{x.Name}")
						.ToList()
				});

			_sessionModel
				.TotalGeneratedCustomers++;
			_queueCustomers.Add(customerModel.Id, queueCustomer);
			
			queueCustomer.StartTimer();
		}

		_customersTimerGenerator.Start();
	}

	private bool CheckTaskCompletion(
		CustomersOrdersSessionModelData modelData) {
		return _sessionModel
				.TotalServedFoodsInOrder
			>= _currentCustomersConfig
				.NeededFoodsInOrderToBeServed
			- 2;
	}

	#region QUEUE_CUSTOMERS_TIMER_CALLBACKS

	private void ONTimerCompleted(CustomerModel obj) {
		_queueCustomers.Remove(obj.Id);
		_customersViewPresenter.RemoveCustomerViewModelById(obj.Id);
	}

	private void ONTimerTicked(CustomerModel arg1, TimeSpan arg2) {
		_customersViewPresenter.RepaintCustomerViewModelTimerById(arg1.Id,
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
			.OrderBy(x => x.TimeLeft)
			.FirstOrDefault();

		if ( queueCustomer == null ) {
			return false;
		}

		_customersViewPresenter.ServeOrderByName(
			queueCustomer.Customer.Id,
			orderModel.Name);

		queueCustomer.Customer.Orders.Remove(orderModel);
		_sessionModel.TotalServedFoodsInOrder++;
		CustomerOrderServed?.Invoke(orderModel);

		if ( !queueCustomer.HasAnyOrders ) {
			_queueCustomers.Remove(queueCustomer.Customer.Id);
			_customersViewPresenter.RemoveCustomerViewModelById(
				queueCustomer.Customer.Id);
			
			_sessionModel.TotalServedCustomers++;
			
			CustomerServed?.Invoke(_sessionModel.TotalServedCustomers, _currentCustomersConfig.TotalCustomersNumber);
		}

		return true;
	}

	#endregion


	// Should be moved outside so we can fetch data from server or internal generator and act like provider via some interface type like ICustomerModelProvider
	private CustomerModel GenerateCustomer() {
		var customer = new CustomerModel() {
			Id = _sessionModel
				.LastCustomerId++,
			IconPath =
				$"Images/Customers/char_{Random.Range(0, TOTAL_CUSTOMERS_ICONS + 1)}",
			Orders = Enumerable
				.Range(0,
					Random.Range(1,
						_currentCustomersConfig.MaxOrdersCount + 1))
				.Select(x
					=> _orderGeneratorService.GenerateRandomOrder())
				.ToList()
		};

		return customer;
	}
}
}