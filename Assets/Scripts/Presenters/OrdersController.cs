using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CookingPrototype.Kitchen;

namespace CookingPrototype.Controllers {
public sealed class OrdersController : MonoBehaviour {
	static OrdersController _instance = null;

	public static OrdersController Instance {
		get {
			if ( !_instance ) {
				_instance = FindObjectOfType<OrdersController>();
			}

			if ( _instance && !_instance._isInit ) {
				_instance.Init();
			}

			return _instance;
		}
		private set { _instance = value; }
	}

	public List<OrderModel> Orders = new List<OrderModel>();

	bool _isInit = false;

	void Awake() {
		if ( (_instance != null) && (_instance != this) ) {
			Debug.LogError(
				"Another instance of OrdersController already exists!");
		}

		Instance = this;
	}

	void OnDestroy() {
		if ( Instance == this ) {
			Instance = null;
		}
	}

	void Start() {
		Init();
	}

	void Init() {
		if ( _isInit ) {
			return;
		}

		var ordersConfig = Resources.Load<TextAsset>("Configs/Orders");
		var ordersXml = new XmlDocument();
		using ( var reader = new StringReader(ordersConfig.ToString()) ) {
			ordersXml.Load(reader);
		}

		var rootElem = ordersXml.DocumentElement;
		foreach ( XmlNode node in rootElem.SelectNodes("order") ) {
			var order = ParseOrder(node);
			Orders.Add(order);
		}

		_isInit = true;
	}

	OrderModel ParseOrder(XmlNode node) {
		var foods =
			(from XmlNode foodNode in node.SelectNodes("food")
				select new OrderModel.OrderFood(foodNode.InnerText,
					foodNode.SelectSingleNode("@needs")?.InnerText))
			.ToList();
		return new OrderModel(node.SelectSingleNode("@name").Value, foods);
	}

	public OrderModel FindOrder(List<string> foods) {
		return Orders.Find(x => {
			if ( x.Foods.Count != foods.Count ) {
				return false;
			}

			return x.Foods.All(food
				=> x.Foods.Count(f => f.Name == food.Name)
				== foods.Count(f => f == food.Name));
		});
	}
}
}