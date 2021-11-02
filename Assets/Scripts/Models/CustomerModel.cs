using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Kitchen;
using UnityEngine;

namespace CookingPrototype.Models {
public class CustomerModel
{
	public int Id { get; set; }
	public List<OrderModel> Orders { get; set; }
	public string IconPath { get; set; }
}
}

