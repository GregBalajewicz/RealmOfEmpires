using System;

namespace Gmbc.Common
{
	/// <summary>
	/// Perform different operations like 'equals' and such 
	/// </summary>
	public class SmartOperations
	{
		private SmartOperations()
		{
		}


		/// <summary>
		/// returns true if value is of the specified type, false otherwise
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsOfType(object value, Type type) 
		{
			if (value.GetType() == type) 
			{
				return true;
			}
			return false;

		}

		public static bool SmartEquals(object value1, object value2, System.Type type1, System.Type type2)
		{
			bool evaluationResult = false;
			if(type1 == type2)
			{
				switch(type1.ToString())
				{
					case "System.Int32":
						try
						{
							evaluationResult = (Convert.ToInt32(value1) == Convert.ToInt32(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type Int32",e);
						}
						break;
					case "System.Int16":
						try
						{
							evaluationResult = ( Convert.ToInt16(value1) == Convert.ToInt16(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type Int16",e);
						}
						break;
					case "System.Boolean":
						try
						{
							evaluationResult = ( Convert.ToBoolean(value1) == Convert.ToBoolean(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type Boolean",e);
						}
						break;
					case "System.Char":
						try
						{
							evaluationResult = ( Convert.ToChar(value1) == Convert.ToChar(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type Char",e);
						}
						break;
					case "System.String":
						try
						{
							evaluationResult = ( Convert.ToString(value1) == Convert.ToString(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type String",e);
						}
						break;
					case "System.Decimal": 
						try
						{
							evaluationResult = ( Convert.ToDecimal(value1) == Convert.ToDecimal(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type Decimal",e);
						}
						break;
					case "System.Double": 
						try
						{
							evaluationResult = ( Convert.ToDouble(value1) == Convert.ToDouble(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type Double",e);
						}
						break;
					case "System.DateTime": 
						try
						{
							evaluationResult = ( Convert.ToDateTime(value1) == Convert.ToDateTime(value2));
						}
						catch(Exception e)
						{
							throw new Exception("EvaluateOperator: Can't convert objects in type DateTime",e);
						}
						break;
					default: 
						throw new ArgumentException("Unrecognized type " + type1.ToString(), "System.Type type1");
				}
				
			}
			return evaluationResult;
		}


		/// <summary>
		/// Check if value1 >= value2
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public static bool SmartGreaterThanOrEquals(object value1, object value2, System.Type type1, System.Type type2)
		{
			if (!IsOfType(value1, type1)) 
			{
				throw new InvalidCastException("value1 is of invalid type. Expecting " + type1.FullName + "(as specified by type1 parameter) but value1 is of type " + value1.GetType().FullName);
			}
			if (!IsOfType(value2, type2)) 
			{
				throw new InvalidCastException("value2 is of invalid type. Expecting " + type2.FullName + "(as specified by type2 parameter) but value2 is of type " + value2.GetType().FullName);
			}
			bool evaluationResult = false;
			if(type1 == type2)
			{
				switch(type1.ToString())
				{
					case "System.Int32":
						evaluationResult = (Convert.ToInt32(value1) >= Convert.ToInt32(value2));
						break;
					case "System.Int16":
						evaluationResult = ( Convert.ToInt16(value1) >= Convert.ToInt16(value2));
						break;
					case "System.Int64":
						evaluationResult = ( ((Int64)value1) >= ((Int64)(value2)));
						break;
					case "System.DateTime":
						evaluationResult = ( ((DateTime)value1) >= ((DateTime)(value2)));
						break;
					case "System.Double":
						evaluationResult = ( ((Double)value1) >= ((Double)(value2)));
						break;
					case "System.Decimal":
						evaluationResult = ( ((Decimal)value1) >= ((Decimal)(value2)));
						break;
					case "System.Char":
						evaluationResult = ( ((Char)value1) >= ((Char)(value2)));
						break;
					default: 
						throw new ArgumentException("Unrecognized type " + type1.ToString() + ". Do not know how to perform >= operation on this type", "System.Type type1");
				}
				
			}
			return evaluationResult;
		}




		
		/// <summary>
		/// Check if value1 (is less then or equal to) value2
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public static bool SmartLessThanOrEquals(object value1, object value2, System.Type type1, System.Type type2)
		{
			if (!IsOfType(value1, type1)) 
			{
				throw new InvalidCastException("value1 is of invalid type. Expecting " + type1.FullName + "(as specified by type1 parameter) but value1 is of type " + value1.GetType().FullName);
			}
			if (!IsOfType(value2, type2)) 
			{
				throw new InvalidCastException("value2 is of invalid type. Expecting " + type2.FullName + "(as specified by type2 parameter) but value2 is of type " + value2.GetType().FullName);
			}
			bool evaluationResult = false;
			if(type1 == type2)
			{
				switch(type1.ToString())
				{
					case "System.Int32":
						evaluationResult = (Convert.ToInt32(value1) <= Convert.ToInt32(value2));
						break;
					case "System.Int16":
						evaluationResult = ( Convert.ToInt16(value1) <= Convert.ToInt16(value2));
						break;
					case "System.Int64":
						evaluationResult = ( ((Int64)value1) <= ((Int64)(value2)));
						break;
					case "System.DateTime":
						evaluationResult = ( ((DateTime)value1) <= ((DateTime)(value2)));
						break;
					case "System.Double":
						evaluationResult = ( ((Double)value1) <= ((Double)(value2)));
						break;
					case "System.Decimal":
						evaluationResult = ( ((Decimal)value1) <= ((Decimal)(value2)));
						break;
					case "System.Char":
						evaluationResult = ( ((Char)value1) <= ((Char)(value2)));
						break;
					default: 
						throw new ArgumentException("Unrecognized type " + type1.ToString() + ". Do not know how to perform <= operation on this type", "System.Type type1");
				}
				
			}
			return evaluationResult;
		}



	}
}
