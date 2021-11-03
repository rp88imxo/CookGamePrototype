using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.GameCore;
using CookingPrototype.Kitchen.Views;
using CookingPrototype.Utilities;
using UnityEngine;

namespace CookingPrototype.Kitchen.Handlers {
public class BurgerPanConfig {
	public int TotalPlaces { get; set; }
	public int BurgerCookTime { get; set; }
	public int BurgerOvercookTime { get; set; }
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

		Timer = new Timer(CookTime.ToTimeSpanSeconds(),
				(1 / 60f).ToTimeSpanSeconds())
			.OnTick(OnTimerTicked)
			.OnCompleted(OnTimerCompleted)
			.SetTickCallbackOnStarted();
	}

	#region TIMER_CALLBACKS_WRAPPERS

	private void OnTimerCompleted() {
		CurrentFood.CookStep();

		switch ( CurrentFood.CurStatus ) {
			case Food.FoodStatus.Cooked:
				_onFoodCooked?.Invoke(this);
				Timer.Reset();
				Timer.Start();
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

public class BurgersPanHandler : MonoBehaviour {
	[SerializeField]
	private CookingFoodView _burgerCutletOnPanViewPrefab;

	[SerializeField]
	private FoodPlaceNew _burgerCutletPlacer;

	[SerializeField]
	private List<Transform> _burgerPanPlaces;

	[SerializeField]
	private SpawnPlacesHandler _burgerCutletSpawnPlacesHandler;

	private BurgerPanConfig _currentBurgerPanConfig;

	private Dictionary<FoodViewModelHandler, CookingFoodView> _foodViews =
		new Dictionary<FoodViewModelHandler, CookingFoodView>();

	private Action<Food> _onServeClicked;
	
	private void Awake() {
		_burgerCutletPlacer.Init(OnCutletPlaceClickedCallback);
	}

	public void Init(BurgerPanConfig burgerPanConfig, Action<Food> onServeClickedCallback) {
		_onServeClicked = onServeClickedCallback;
		_currentBurgerPanConfig = burgerPanConfig;
		
		_burgerPanPlaces.ForEach(x => x.gameObject.SetActive(false));
		var totalActivePlaces = _burgerPanPlaces
			.Take(burgerPanConfig.TotalPlaces)
			.ToList();
		totalActivePlaces.ForEach(x => x.gameObject.SetActive(true));
		_burgerCutletSpawnPlacesHandler.AddSpawnPoints(totalActivePlaces);
	}

	private void OnCutletPlaceClickedCallback(Food obj) {
		if ( !_burgerCutletSpawnPlacesHandler.HasAnyFreeSpawnPoint ) {
			Debug.Log("No places to place a Cutlet!");
			return;
		}

		var spawnPoint = _burgerCutletSpawnPlacesHandler.GetSpawnPoint();

		var foodViewModelHandler = new FoodViewModelHandler(obj,
			_currentBurgerPanConfig.BurgerCookTime,
			_currentBurgerPanConfig.BurgerOvercookTime,
			ONFoodCooked,
			ONTimerTicked,
			ONFoodOvercooked);

		var go = Instantiate(_burgerCutletOnPanViewPrefab, spawnPoint);
		go.Init(foodViewModelHandler, ONServeClicked, ONTrashClicked);
		go.Repaint(new CookingFoodViewModel {
			FoodViewState = obj.CurStatus,
			CookTime = _currentBurgerPanConfig.BurgerCookTime
		});

		_foodViews.Add(foodViewModelHandler, go);

		foodViewModelHandler.StartTimer();
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
		if ( obj.CurrentFood.CurStatus == Food.FoodStatus.Cooked ) {
			_onServeClicked?.Invoke(obj.CurrentFood);
		}
	}

	#endregion

	#region BURGER_CUTLET_ON_PAN_TIMER_CALLBACKS

	private void ONFoodCooked(FoodViewModelHandler obj) {
		var cookingFoodView = _foodViews[obj];
		cookingFoodView.Repaint(new CookingFoodViewModel() {
			FoodViewState = obj.CurrentFood.CurStatus,
			CookTime = _currentBurgerPanConfig.BurgerOvercookTime
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
	
}
}