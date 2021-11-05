using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using UnityEngine;
using UnityEngine.Serialization;

namespace CookingPrototype.Kitchen.Handlers {
public class ColaFoodHandler : MonoBehaviour
{
	[SerializeField]
	private CookingFoodView _cookingFoodViewPrefab;

	[SerializeField]
	private BaseFoodPlacerHandler _foodPlacerHandler;

	[SerializeField]
	private List<Transform> _foodPlaces;

	[SerializeField]
	private SpawnPlacesHandler _spawnPlacesHandler;

	private CookableFoodConfig _currentCookableFoodConfig;

	private readonly Dictionary<FoodViewModelHandler, CookingFoodView> _foodViews =
		new Dictionary<FoodViewModelHandler, CookingFoodView>();

	private Func<Food,bool> _onServedRequested;
	public bool HasFreePlaces => _spawnPlacesHandler.HasAnyFreeSpawnPoint;

	private BaseOrderAssemblyHandler _colaAssembleHandler;
	public event Action CookplaceStateUpdated;

	public void Init(CookableFoodConfig cookableFoodConfig,
		Func<Food, bool> onServeRequestedCallback,
		BaseOrderAssemblyHandler colaAssembleHandler) {
		_colaAssembleHandler = colaAssembleHandler;
		_colaAssembleHandler.OrderServed += ColaAssembleHandlerOnOrderServed;
		_foodPlacerHandler.Init();
		_foodPlacerHandler.OnFoodPlaced += OnTryFoodPlaceClickedCallback;
		
		_onServedRequested = onServeRequestedCallback;
		_currentCookableFoodConfig = cookableFoodConfig;
		
		_foodPlaces.ForEach(x => x.gameObject.SetActive(false));
		var totalActivePlaces = _foodPlaces
			.Take(cookableFoodConfig.TotalPlaces)
			.ToList();
		totalActivePlaces.ForEach(x => x.gameObject.SetActive(true));
		_spawnPlacesHandler.AddSpawnPoints(totalActivePlaces);
		CookplaceStateUpdated?.Invoke();
	}

	private void ColaAssembleHandlerOnOrderServed() {
		RemoveView();
	}

	private bool OnTryFoodPlaceClickedCallback(Food obj) {
		if ( !_spawnPlacesHandler.HasAnyFreeSpawnPoint ) {
			Debug.Log("No places to place a Cutlet!");
			return false;
		}

		var spawnPoint = _spawnPlacesHandler.GetSpawnPoint();

		var foodViewModelHandler = new FoodViewModelHandler(obj,
			_currentCookableFoodConfig.CookTime,
			_currentCookableFoodConfig.OvercookTime,
			ONFoodCooked,
			ONTimerTicked,
			null);

		var go = Instantiate(_cookingFoodViewPrefab, spawnPoint);
		go.Init(foodViewModelHandler, null, null);
		go.Repaint(new CookingFoodViewModel {
			FoodViewState = obj.CurStatus,
			CookTime = _currentCookableFoodConfig.CookTime
		});
		
		_foodViews.Add(foodViewModelHandler, go);
		foodViewModelHandler.StartTimer();
		CookplaceStateUpdated?.Invoke();
		
		return true;
	}

	#region INTERACTION_CALLBACKS
	
	private void ONServeRequested(FoodViewModelHandler obj) {
		if ( obj.CurrentFood.CurStatus != Food.FoodStatus.Cooked ) {
			return;
		}

		var res=	_onServedRequested?.Invoke(obj.CurrentFood);
		if ( res.HasValue && res.Value) 
		{
			HideView(obj);
		}
	}

	#endregion

	#region TIMER_CALLBACKS
	
	private void ONFoodCooked(FoodViewModelHandler obj) {
		var cookingFoodView = _foodViews[obj];
		cookingFoodView.Repaint(new CookingFoodViewModel() {
			FoodViewState = obj.CurrentFood.CurStatus
		});
		cookingFoodView.ToogleTimer(false);
		obj.StopTimer();
		ONServeRequested(obj);
	}
	

	private void ONTimerTicked(FoodViewModelHandler arg1, TimeSpan arg2) {
		var cookingFoodView = _foodViews[arg1];
		cookingFoodView.RepaintTimer((float)arg2.TotalSeconds);
	}

	#endregion

	private void RemoveView(FoodViewModelHandler foodViewModelHandler) {
		var cookingView = _foodViews[foodViewModelHandler];
		cookingView.DestroySelf();
		foodViewModelHandler.StopTimer();
		_foodViews.Remove(foodViewModelHandler);
		CookplaceStateUpdated?.Invoke();
	}
	
	private void RemoveView() {
		var foodViewModelHandler = _foodViews.FirstOrDefault(x
			=> x.Key.CurrentFood.CurStatus == Food.FoodStatus.Cooked);
		RemoveView(foodViewModelHandler.Key);
	}
	
	private void HideView(FoodViewModelHandler foodViewModelHandler) {
		var cookingView = _foodViews[foodViewModelHandler];
		foodViewModelHandler.StopTimer();
		cookingView.Toogle(false);
	}

	public void HandleSessionEnded() {
		_colaAssembleHandler.OrderServed -= ColaAssembleHandlerOnOrderServed;
		_foodPlacerHandler.OnFoodPlaced -= OnTryFoodPlaceClickedCallback;
		
		foreach ( var food in _foodViews.Keys ) {
			var cookingView = _foodViews[food];
			cookingView.DestroySelf();
			food.StopTimer();
		}
		
		_spawnPlacesHandler.RemoveAllPoints();
		_foodViews.Clear();
	}
}
}

