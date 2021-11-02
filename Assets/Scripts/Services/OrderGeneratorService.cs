using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CookingPrototype.Kitchen;
using UnityEngine;

namespace CookingPrototype.Services {
public class OrderGeneratorService {

	private readonly List<OrderModel> _orders;
	
	public OrderGeneratorService() {
		_orders = new List<OrderModel>();
	}

	public void Init(string orderConfigPath = "Configs/Orders") {
		_orders.Clear();
		var ordersConfig = Resources.Load<TextAsset>(orderConfigPath);
		var ordersXml = new XmlDocument();
		using ( var reader = new StringReader(ordersConfig.ToString()) ) {
			ordersXml.Load(reader);
		}

		var rootElem = ordersXml.DocumentElement;
		foreach ( XmlNode node in rootElem.SelectNodes("order") ) {
			var order = ParseOrder(node);
			_orders.Add(order);
		}
	}
	
	public OrderModel GenerateRandomOrder() 
		=> _orders[Random.Range(0, _orders.Count)];
	
	private OrderModel ParseOrder(XmlNode node) {
		var foods =
			(from XmlNode foodNode in node.SelectNodes("food")
				select new OrderModel.OrderFood(foodNode.InnerText,
					foodNode.SelectSingleNode("@needs")?.InnerText))
			.ToList();
		return new OrderModel(node.SelectSingleNode("@name").Value, foods);
	}
}
}

