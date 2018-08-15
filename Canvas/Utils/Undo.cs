using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Canvas
{
    /// <summary>
    /// 编辑命令基础
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    class EditCommandBase
	{
        /// <summary>
        /// 执行撤回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public virtual bool DoUndo(IModel data)
		{
			return false;
		}

        /// <summary>
        /// 执行重做
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public virtual bool DoRedo(IModel data)
		{
			return false;
		}
	}
    /// <summary>
    /// 编辑命令:添加
    /// </summary>
	class EditCommandAdd : EditCommandBase
	{
		List<IDrawObject> m_objects = null;
		IDrawObject m_object;
		ICanvasLayer m_layer;
        /// <summary>
        /// 编辑命令添加
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="obj"></param>
		public EditCommandAdd(ICanvasLayer layer, IDrawObject obj)
		{
			m_object = obj;
			m_layer = layer;
		}
        /// <summary>
        /// 编辑命令添加
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="objects"></param>
		public EditCommandAdd(ICanvasLayer layer, List<IDrawObject> objects)
		{
			m_objects = new List<IDrawObject>(objects);
			m_layer = layer;
		}

        /// <summary>
        /// 执行撤回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoUndo(IModel data)
		{
			if (m_object != null)
				data.DeleteObjects(new IDrawObject[] { m_object });
			if (m_objects != null)
				data.DeleteObjects(m_objects);
			return true;
		}
        /// <summary>
        /// 执行重做
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoRedo(IModel data)
		{
			if (m_object != null)
				data.AddObject(m_layer, m_object);
			if (m_objects != null)
			{
				foreach (IDrawObject obj in m_objects)
					data.AddObject(m_layer, obj);
			}
			return true;
		}
	}

    /// <summary>
    /// 编辑命令:移除
    /// </summary>
	class EditCommandRemove : EditCommandBase
	{
		Dictionary<ICanvasLayer, List<IDrawObject>> m_objects = new Dictionary<ICanvasLayer, List<IDrawObject>>();
		public EditCommandRemove()
		{
		}
        /// <summary>
        /// 向字典中添加图层信息和图形对象列表
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="objects"></param>
		public void AddLayerObjects(ICanvasLayer layer, List<IDrawObject> objects)
		{
			m_objects.Add(layer, objects);
		}

        /// <summary>
        /// 执行撤回,循环向data中添加图层信息和图形对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoUndo(IModel data)
		{
			foreach (ICanvasLayer layer in m_objects.Keys)
			{
				foreach (IDrawObject obj in m_objects[layer])
					data.AddObject(layer, obj);
			}
			return true;
		}
        /// <summary>
        /// 执行重写
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoRedo(IModel data)
		{
			foreach (ICanvasLayer layer in m_objects.Keys)
				data.DeleteObjects(m_objects[layer]);
			return true;
		}
	}
    /// <summary>
    /// 编辑命令:移动
    /// </summary>
	class EditCommandMove : EditCommandBase
	{
		List<IDrawObject> m_objects = new List<IDrawObject>();
		UnitPoint m_offset;
        /// <summary>
        /// 编辑命令:移动
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="objects"></param>
		public EditCommandMove(UnitPoint offset, IEnumerable<IDrawObject> objects)
		{
            //MessageBox.Show("");
			m_objects = new List<IDrawObject>(objects);
			m_offset = offset;
		}
        /// <summary>
        /// 执行撤回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoUndo(IModel data)
		{
			foreach (IDrawObject obj in m_objects)
			{
				UnitPoint offset = new UnitPoint(-m_offset.X, -m_offset.Y);
				obj.Move(offset);
			}
			return true;
		}
        /// <summary>
        /// 执行重做
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoRedo(IModel data)
		{
			foreach (IDrawObject obj in m_objects)
				obj.Move(m_offset);
			return true;
		}
	}

    /// <summary>
    /// 编辑命令:端点移动
    /// </summary>
	class EditCommandNodeMove : EditCommandBase
	{
		List<INodePoint> m_objects = new List<INodePoint>();
        /// <summary>
        /// 编辑命令:移动
        /// </summary>
        /// <param name="objects"></param>
		public EditCommandNodeMove(IEnumerable<INodePoint> objects)
		{
			m_objects = new List<INodePoint>(objects);
		}
        /// <summary>
        /// 执行撤回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoUndo(IModel data)
		{
			foreach (INodePoint obj in m_objects)
				obj.Undo();
			return true;
		}
        /// <summary>
        /// 执行重做
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoRedo(IModel data)
		{
			foreach (INodePoint obj in m_objects)
				obj.Redo();
			return true;
		}
	}
    /// <summary>
    /// 编辑工具:编辑命令
    /// </summary>
	class EditCommandEditTool : EditCommandBase
	{
		IEditTool m_tool;
        /// <summary>
        /// 编辑工具:编辑命令
        /// </summary>
        /// <param name="tool"></param>
		public EditCommandEditTool(IEditTool tool)
		{
            //MessageBox.Show("");
			m_tool = tool;
		}
        /// <summary>
        /// 执行撤回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoUndo(IModel data)
		{
			m_tool.Undo();
			return true;
		}
        /// <summary>
        /// 执行重做
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public override bool DoRedo(IModel data)
		{
			m_tool.Redo();
			return true;
		}
	}
    /// <summary>
    /// 撤回重做缓冲
    /// </summary>
	class UndoRedoBuffer
	{
		List<EditCommandBase> m_undoBuffer = new List<EditCommandBase>();
		List<EditCommandBase> m_redoBuffer = new List<EditCommandBase>();
		bool m_canCapture = true;
		bool m_dirty = false;
		public UndoRedoBuffer()
		{
		}
        /// <summary>
        /// 数据清空
        /// </summary>
		public void Clear()
		{
			m_undoBuffer.Clear();
			m_redoBuffer.Clear();
		}
        /// <summary>
        /// m_dirty
        /// </summary>
		public bool Dirty
		{
			get { return m_dirty; }
			set { m_dirty = value;}
		}

        /// <summary>
        /// m_canCapture
        /// </summary>
		public bool CanCapture
		{
			get { return m_canCapture; }
		}

        /// <summary>
        /// 能否撤回,m_undoBuffer.Count
        /// </summary>
		public bool CanUndo
		{
			get { return m_undoBuffer.Count > 0; }
		}
        /// <summary>
        /// 能否重做,m_redoBuffer.Count
        /// </summary>
		public bool CanRedo
		{
			get { return m_redoBuffer.Count > 0; }
		}

        /// <summary>
        /// 添加命令(为缓冲添加数据)
        /// </summary>
        /// <param name="command"></param>
		public void AddCommand(EditCommandBase command)
		{
			if (m_canCapture && command != null)
			{
				m_undoBuffer.Add(command);
				m_redoBuffer.Clear();
				Dirty = true;
			}
		}
        /// <summary>
        /// 执行撤回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public bool DoUndo(IModel data)
		{
			if (m_undoBuffer.Count == 0)
				return false;
			m_canCapture = false;
			EditCommandBase command = m_undoBuffer[m_undoBuffer.Count - 1];
			bool result = command.DoUndo(data);
			m_undoBuffer.RemoveAt(m_undoBuffer.Count - 1);
			m_redoBuffer.Add(command);
			m_canCapture = true;
			Dirty = true;
			return result;
		}
        /// <summary>
        /// 执行重做
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public bool DoRedo(IModel data)
		{
			if (m_redoBuffer.Count == 0)
				return false;
			m_canCapture = false;
			EditCommandBase command = m_redoBuffer[m_redoBuffer.Count - 1];
			bool result = command.DoRedo(data);
			m_redoBuffer.RemoveAt(m_redoBuffer.Count - 1);
			m_undoBuffer.Add(command);
			m_canCapture = true;
			Dirty = true;
			return result;
		}
	}
}
