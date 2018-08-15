using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace Canvas
{
    /// <summary>
    /// xml文件序列化
    /// </summary>
	interface ISerialize
	{
        /// <summary>
        /// 从xml中读取所有图形对象
        /// </summary>
        /// <param name="wr"></param>
		void GetObjectData(XmlWriter wr);
        /// <summary>
        /// 序列化读取完成后触发
        /// </summary>
		void AfterSerializedIn();
	}
	public class XmlSerializable : System.Attribute
	{
        /// <summary>
        /// 不明白
        /// </summary>
		public XmlSerializable() { }
	}
	class XmlUtil
	{
        /// <summary>
        /// 添加xml元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="wr"></param>
		public static void AddProperty(string name, object value, XmlWriter wr)
		{
			string svalue = string.Empty;
			if (value is string)
				svalue = value as string;
			if (svalue.Length == 0 && value.GetType() == typeof(float))
				svalue = XmlConvert.ToString(Math.Round((float)value, 8));
			if (svalue.Length == 0 && value.GetType() == typeof(double))
				svalue = XmlConvert.ToString(Math.Round((double)value, 8));
			if (svalue.Length == 0)
				svalue = value.ToString();
			
			wr.WriteStartElement("property");
			wr.WriteAttributeString("name", name);
			wr.WriteAttributeString("value", svalue);
			wr.WriteEndElement();
		}

        /// <summary>
        /// 解析xml元素
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dataobject"></param>
        public static void ParseProperty(XmlElement node, object dataobject)
		{
			if (node.Name != "property")
				return;

			string fieldname = node.GetAttribute("name");
			string svalue = node.GetAttribute("value");
			if (fieldname.Length == 0 || svalue.Length == 0)
				return;

			PropertyInfo info = CommonTools.PropertyUtil.GetProperty(dataobject, fieldname);
			if (info == null || info.CanWrite == false)
				return;
			try
			{
				object value = PropertyUtil.ChangeType(svalue, info.PropertyType);
				if (value != null)
					info.SetValue(dataobject, value, null);
			}
			catch(Exception e) { MessageBox.Show(e.Message); };
		}

        /// <summary>
        /// 解析带有子元素的xml元素
        /// </summary>
        /// <param name="itemnode"></param>
        /// <param name="dataobject"></param>
		public static void ParseProperties(XmlElement itemnode, object dataobject)
		{
			foreach (XmlElement propertynode in itemnode.ChildNodes)
				XmlUtil.ParseProperty(propertynode, dataobject);
		}

        /// <summary>
        /// 写入带有子元素的xml元素
        /// </summary>
        /// <param name="dataobject"></param>
        /// <param name="wr"></param>
        public static void WriteProperties(object dataobject, XmlWriter wr)
		{
			foreach (PropertyInfo propertyInfo in dataobject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				XmlSerializable attr = (XmlSerializable)Attribute.GetCustomAttribute(propertyInfo, typeof(XmlSerializable));
				if (attr != null)
				{
					string name	= propertyInfo.Name;
					object value = propertyInfo.GetValue(dataobject, null);
					if (value != null)
						AddProperty(name, value, wr);
				}
			}
		}
	}

	class PropertyUtil
	{
        /// <summary>
        /// 如果type不为UnitPoint,调用CommonTools.PropertyUtil.ChangeType(value, type)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
		public static object ChangeType(object value, Type type)
		{
			if (type == typeof(UnitPoint))
				return Parse(value.ToString(), type);
			return CommonTools.PropertyUtil.ChangeType(value, type);
		}

        /// <summary>
        /// 解析,CommonTools.PropertyUtil.Parse
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
		static public object Parse(string value, Type type)
		{
			if (type == typeof(UnitPoint))
				return CommonTools.PropertyUtil.Parse(new UnitPoint(0, 0), value);
			return CommonTools.PropertyUtil.Parse(value, type);
		}
	}

	class SerializeUtil
	{

	}
}
