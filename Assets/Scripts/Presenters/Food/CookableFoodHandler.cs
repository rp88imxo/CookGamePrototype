﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace CookingPrototype.Kitchen.Handlers {
public class CookableFoodConfig {
	public int TotalPlaces { get; set; }
	public int CookTime { get; set; }
	public int OvercookTime { get; set; }
}



public class FoodViewModelHandler {
	public Food CurrentFood { get; }
	private Timer Timer { get; set; }

	// public float TimeLeft => WaitTimer != null
	// 	? (float)WaitTimer.TimeLeft.TotalSeconds
	// 	: 0f;

	private Action<FoodViewModelHandler> _onFoodCooked;
	private Action<FoodViewModelHandler, TimeSpan> _onTimerTicked;
	private Action<FoodViewModelHandler> _onFoodOvercooked;

	public float CookTime { get; private set; }
	public float OvercookTime { get; private set; }

	public FoodViewModelHandler(Food currentFood,
		float cookTime,
		float overcookTime,
		Action<FoodViewModelHandler> onFoodCooked,
		Action<FoodViewModelHandler, TimeSpan> onTimerTicked,
		Action<FoodViewModelHandler> onFoodOvercooked) {
		_onFoodCooked = onFoodCooked;
		_onTimerTicked = onTimerTicked;
		_onFoodOvercooked = onFoodOvercooked;

		CurrentFood = currentFood;

		CookTime = cookTime;
		OvercookTime = overcookTime;

		var tickInterval = (1 / 60f).ToTimeSpanSeconds();
		Timer = new Timer(CookTime.ToTimeSpanSeconds(),
				tickInterval)
			.OnTick(OnTimerTicked)
			.OnCompleted(OnTimerCompleted)
			.SetTickCallbackOnStarted()
			.SetTickInterval(tickInterval);
	}

	#region TIMER_CALLBACKS_WRAPPERS

	private void OnTimerCompleted() {
		CurrentFood.CookStep();

		switch ( CurrentFood.CurStatus ) {
			case Food.FoodStatus.Cooked:
				_onFoodCooked?.Invoke(this);
				if ( Timer != null ) {
					Timer.Reset();
					Timer.Start();
				}
				break;
			case Food.FoodStatus.Overcooked:
				_onFoodOvercooked?.Invoke(this);
				break;
		}
	}

	private void OnTimerTicked(TimeSpan obj) {
		_onTimerTicked?.Invoke(this, obj);
	}

	#endregion

	public void StartTimer() {
		Timer?.Start();
	}

	public void StopTimer() {
		Timer?.Stop();
		Timer = null;
	}

	public void PauseTimer() {
		Timer?.Stop();
	}

	public void ResumeTimer() {
		Timer?.Resume();
	}
}

public class CookableFoodHandler : MonoBehaviour {
	[FormerlySerializedAs("_burgerCutletOnPanViewPrefab")]
	[SerializeField]
	private CookingFoodView _cookingFoodViewPrefab;

	[FormerlySerializedAs("_burgerCutletPlacer")]
	[SerializeField]
	private BaseFoodPlacerHandler _foodPlacerHandler;

	[FormerlySerializedAs("_burgerPanPlaces")]
	[SerializeField]
	private List<Transform> _foodPlaces;

	[FormerlySerializedAs("_burgerCutletSpawnPlacesHandler")]
	[SerializeField]
	private SpawnPlacesHandler _spawnPlacesHandler;
	
	
	private CookableFoodConfig _currentCookableFoodConfig;

	private Dictionary<FoodViewModelHandler, CookingFoodView> _foodViews =
		new Dictionary<FoodViewModelHandler, CookingFoodView>();

	private Func<Food,bool> _onServeClicked;
	public bool HasFreePlaces => _spawnPlacesHandler.HasAnyFreeSpawnPoint;

	public void Init(CookableFoodConfig cookableFoodConfig, Func<Food,bool> onServeClickedCallback) {
		_foodPlacerHandler.Init();
		_foodPlacerHandler.OnFoodPlaced += OnCutletPlaceClickedCallback;
		
		_onServeClicked = onServeClickedCallback;
		_currentCookableFoodConfig = cookableFoodConfig;
		
		_foodPlaces.ForEach(x => x.gameObject.SetActive(false));
		var totalActivePlaces = _foodPlaces
			.Take(cookableFoodConfig.TotalPlaces)
			.ToList();
		totalActivePlaces.ForEach(x => x.gameObject.SetActive(true));
		_spawnPlacesHandler.AddSpawnPoints(totalActivePlaces);
	}

	private bool OnCutletPlaceClickedCallback(Food obj) {
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
			ONFoodOvercooked);

		var go = Instantiate(_cookingFoodViewPrefab, spawnPoint);
		go.Init(foodViewModelHandler, ONServeClicked, ONTrashClicked);
		go.Repaint(new CookingFoodViewModel {
			FoodViewState = obj.CurStatus,
			CookTime = _currentCookableFoodConfig.CookTime
		});

	
		_foodViews.Add(foodViewModelHandler, go);

		foodViewModelHandler.StartTimer();

		return true;
	}

	#region BURGER_CUTLET_INTERACTION_CALLBACKS

	private void ONTrashClicked(FoodViewModelHandler obj) {
		if ( obj.CurrentFood.CurStatus == Food.FoodStatus.Overcooked ) {
			var cookingView = _foodViews[obj];
			cookingView.DestroySelf();
			obj.StopTimer();
			_foodViews.Remove(obj);
		}
	}

	private void ONServeClicked(FoodViewModelHandler obj) {
		if ( obj.CurrentFood.CurStatus != Food.FoodStatus.Cooked ) {
			return;
		}

		var res=	_onServeClicked?.Invoke(obj.CurrentFood);
		if ( res.HasValue && res.Value) 
		{
			RemoveView(obj);
		}
	}

	#endregion

	#region BURGER_CUTLET_ON_PAN_TIMER_CALLBACKS

	private void ONFoodCooked(FoodViewModelHandler obj) {
		var cookingFoodView = _foodViews[obj];
		cookingFoodView.Repaint(new CookingFoodViewModel() {
			FoodViewState = obj.CurrentFood.CurStatus,
			CookTime = _currentCookableFoodConfig.OvercookTime
		});
	}

	private void ONFoodOvercooked(FoodViewModelHandler obj) {
		var cookingFoodView = _foodViews[obj];
		cookingFoodView.Repaint(new CookingFoodViewModel {
			FoodViewState = obj.CurrentFood.CurStatus,
		});

		//cookingFoodView.ToogleTimer(false);
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
	}

	public void HandleSessionEnded() {
		_foodPlacerHandler.OnFoodPlaced -= OnCutletPlaceClickedCallback;
		
		foreach ( var food in _foodViews.Keys ) {
			var cookingView = _foodViews[food];
			cookingView.DestroySelf();
			food.StopTimer();
		}
		
		_foodViews.Clear();
	}
}
}