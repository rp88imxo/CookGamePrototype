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
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CookingPrototype.Kitchen.Controllers {
public class CustomersConfig {
	public int TotalCustomersNumber { get; set; } = 15;
	public float CustomerWaitTime { get; set; } = 18f;
	public float CustomerSpawnTime { get; set; } = 3f;
	public int MaxOrdersCount { get; set; } = 3;
	public float AddedTimeOnServedOrder { get; set; } = 6f;
	public List<List<OrderModel>> LevelOrders { get; set; }

	public int NeededFoodsInOrderToBeServed => LevelOrders
			.SelectMany(x => x)
			.Count()
		- 2;
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
		var tickInterval = (1 / 60f).ToTimeSpanSeconds();
		WaitTimer = new Timer(initialTime, tickInterval)
			.OnStarted(OnTimerStarted)
			.OnTick(OnTimerTicked)
			.OnCompleted(OnTimerCompleted)
			.SetTickCallbackOnStarted()
			.SetTickInterval(tickInterval);
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

	public void AddTime(float f) {
		WaitTimer.TimeLeft = Mathf
			.Min((float)WaitTimer.TimeLeft.TotalSeconds + f,
				(float)WaitTimer.InitialTime.TotalSeconds)
			.ToTimeSpanSeconds();
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
	/// Customer generated
	/// </summary>
	public static event Action CustomerGenerated;

	/// <summary>
	/// Customer fully served
	/// </summary>
	public static event Action CustomerServed;

	/// <summary>
	/// Customer left from the queue
	/// </summary>
	public static event Action CustomerLeft;

	/// <summary>
	/// Served some order(part of the full order) of customer
	/// </summary>
	public static event Action<OrderModel> CustomerOrderServed;

	/// <summary>
	/// Called when we hit some condition to end the session
	/// </summary>
	public static event Action<bool> CustomerTaskCompleted;

	public int TotalServedOrders
		=> _sessionModel.TotalServedFoodsInOrder;

	public int TotalTargetOrders
		=> _currentCustomersConfig.NeededFoodsInOrderToBeServed;

	public int CustomersLeft
		=> _currentCustomersConfig.TotalCustomersNumber
			- _sessionModel.TotalGeneratedCustomers;

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

		Debug.Assert(
			config.TotalCustomersNumber == config.LevelOrders.Count,
			"Total customers and total level orders dimensions should be equal!");

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

		foreach ( var customer in _queueCustomers.Values ) {
			customer.StopTimer();
		}

		_queueCustomers.Clear();
		_customersViewPresenter.HandleSessionEnd();
		_customersTimerGenerator.Stop();
	}

	#region EVENTS_CALLBACKS

	private void OnCustomerUpdated() {
		if ( IsCustomerTaskCompleted() ) {
			CustomerTaskCompleted?.Invoke(CheckTaskCompletion());
		}
	}

	private void OnCustomerOrderServed(OrderModel obj) {
		if ( _sessionModel.TotalServedFoodsInOrder
			== _currentCustomersConfig
				.NeededFoodsInOrderToBeServed ) {
			CustomerTaskCompleted?.Invoke(true);
		}
	}

	#endregion

	private bool IsCustomerTaskCompleted() {
		return _sessionModel
				.TotalGeneratedCustomers
			== _currentCustomersConfig.TotalCustomersNumber
			&& _queueCustomers.Count == 0;
	}

	private void TryGenerateCustomer() {
		_customersTimerGenerator.Reset();

		if ( _customersViewPresenter.HasFreeSpawnPoint
			&& CanGenerateCustomer() ) {
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
					OrdersViewsNames = customerModel.Orders
						.Select(x => x.Name)
						.ToList()
				});

			_sessionModel
				.TotalGeneratedCustomers++;
			_queueCustomers.Add(customerModel.Id, queueCustomer);

			queueCustomer.StartTimer();

			CustomerGenerated?.Invoke();
		}

		_customersTimerGenerator.Start();
	}

	private bool CanGenerateCustomer() {
		return _sessionModel.TotalGeneratedCustomers
			< _currentCustomersConfig.TotalCustomersNumber;
	}

	private bool CheckTaskCompletion() {
		return _sessionModel
				.TotalServedFoodsInOrder
			>= _currentCustomersConfig
				.NeededFoodsInOrderToBeServed;
	}

	#region QUEUE_CUSTOMERS_TIMER_CALLBACKS

	private void ONTimerCompleted(CustomerModel obj) {
		_queueCustomers.Remove(obj.Id);
		_customersViewPresenter.RemoveCustomerViewModelById(obj.Id);
		CustomerLeft?.Invoke();
		OnCustomerUpdated();
	}

	private void ONTimerTicked(CustomerModel arg1, TimeSpan arg2) {
		_customersViewPresenter.RepaintCustomerViewModelTimerById(arg1.Id,
			arg2);
	}

	#endregion

	#region SOME_PUBLIC_API

	public void ServeOrder(OrderModel orderModel,
		Action onOrderServeSucceeded,
		Action onOrderServeFailed) {
		var queueCustomer = _queueCustomers.Values
			.Where(x
				=> x.Customer.Orders.Any(order
					=> order.Equals(orderModel)))
			.OrderBy(x => x.TimeLeft)
			.FirstOrDefault();

		if ( queueCustomer == null ) {
			onOrderServeFailed?.Invoke();
			return;
		}

		_customersViewPresenter.ServeOrderByName(
			queueCustomer.Customer.Id,
			orderModel.Name);

		queueCustomer.Customer.Orders.Remove(orderModel);
		_sessionModel.TotalServedFoodsInOrder++;

		queueCustomer.AddTime(_currentCustomersConfig
			.AddedTimeOnServedOrder);

		onOrderServeSucceeded?.Invoke();
		CustomerOrderServed?.Invoke(orderModel);
		OnCustomerOrderServed(orderModel);

		if ( !queueCustomer.HasAnyOrders ) {
			queueCustomer.StopTimer();
			_queueCustomers.Remove(queueCustomer.Customer.Id);
			_customersViewPresenter.RemoveCustomerViewModelById(
				queueCustomer.Customer.Id);

			_sessionModel.TotalServedCustomers++;

			CustomerServed?.Invoke();
			OnCustomerUpdated();
		}
	}

	#endregion


	private CustomerModel GenerateCustomer() {
		var customer = new CustomerModel() {
			Id = _sessionModel
				.LastCustomerId++,
			IconPath =
				$"Images/Customers/char_{Random.Range(1, TOTAL_CUSTOMERS_ICONS + 1)}",
			Orders = GetOrders(_currentCustomersConfig.LevelOrders)
		};

		return customer;
	}

	private List<OrderModel>
		GetOrders(List<List<OrderModel>> levelOrders) {
		return levelOrders[_sessionModel.TotalGeneratedCustomers].Clone();
	}
}
}