using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {
public class GameplayMainScreenView : View {
	[SerializeField]
	private CustomersViewPresenter _customersViewPresenter;

	[SerializeField]
	private FoodViewPresenter _foodViewPresenter;

	[SerializeField]
	private GameplayTopUIView _gameplayTopUIView;

	[SerializeField]
	private GameplayResultWindowView _winWindowView;

	[SerializeField]
	private GameplayResultWindowView _loseWindowView;

	[SerializeField]
	private GameplayResultWindowView _startWindowView;

	
	public GameplayResultWindowView StartWindowView => _startWindowView;
	public GameplayResultWindowView WinWindowView => _winWindowView;
	public GameplayResultWindowView LoseWindowView => _loseWindowView;
	public GameplayTopUIView GameplayTopUIView => _gameplayTopUIView;
	public FoodViewPresenter FoodViewPresenter => _foodViewPresenter;

	public CustomersViewPresenter CustomersViewPresenter
		=> _customersViewPresenter;
}
}