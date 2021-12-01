using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace ShaderForm2
{
	public class PersistentSettings
	{
		public void Load()
		{
			if (File.Exists(fileName))
			{
				//load values from file
				var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(fileName));
				if (dic is not null) foreach ((string key, object value) in dic)
					{
						settings[key].Setter(value);
					}
			}
		}

		public void AddProperty<TType>(string name, Func<TType> getter, Action<TType> setter)
		{
			//TODO: if(settings.ContainsKey(name))
			settings.Add(name, new Prop(() => getter(), value => setter((TType)value)));
}

		public void AddProperty<ValueType>(Expression<Func<ValueType>> propertyExpression)
		{
			if (propertyExpression.Body is MemberExpression memberExpression)
			{
				if (memberExpression?.Member is PropertyInfo propertyInfo)
				{
					if (memberExpression.Expression is null) throw new ArgumentException("Invalid expression given");
					var instance = Evaluate(memberExpression.Expression);
					AddProperty(propertyInfo.Name, () => propertyInfo.GetValue(instance), value => propertyInfo.SetValue(instance, value));
					return;
				}
			}
			throw new InvalidOperationException("Please provide a valid property expression, like '() => instance.PropertyName'.");
		}

		public void Store()
		{
			var dic = settings.ToDictionary(prop => prop.Key, prop => prop.Value.Getter());
			var text = JsonConvert.SerializeObject(dic);
			File.WriteAllText(fileName, text);
		}

		private struct Prop
		{
			public Func<object?> Getter { get; }
			public Action<object> Setter { get; }

			public Prop(Func<object?> getter, Action<object> setter) : this()
			{
				Getter = getter;
				Setter = setter;
			}
		}

		private readonly Dictionary<string, Prop> settings = new();
		private readonly string fileName = Path.ChangeExtension(Assembly.GetCallingAssembly().Location, "config.json");

		private static object? Evaluate(Expression e)
		{
			switch (e.NodeType)
			{
				case ExpressionType.Constant:
					return (e as ConstantExpression)?.Value;
				case ExpressionType.MemberAccess:
					{
						if (e is not MemberExpression propertyExpression) return null;
						var field = propertyExpression.Member as FieldInfo;
						var property = propertyExpression.Member as PropertyInfo;
						var container = propertyExpression.Expression == null ? null : Evaluate(propertyExpression.Expression);
						if (field != null)
							return field.GetValue(container);
						else if (property != null)
							return property.GetValue(container, null);
						else
							return null;
					}
				default:
					return null;
			}
		}
	}
}
