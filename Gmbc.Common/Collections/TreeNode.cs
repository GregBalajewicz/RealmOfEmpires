using System;
using System.Collections;
using System.Collections.Specialized;

namespace Gmbc.Common.Collections
{



	/// <summary>
	/// A TreeNode object represents a node in a Tree structure / collection.
	/// It was originally created for the Permission component
	/// </summary>
	public class TreeNode :Gmbc.Common.GmbcBaseClass, Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection
	{
		private string nodeID;	
		private string nodeName; 
		private object nodeData;
		private TreeNode parent;
		private TreeNodeList children;
		private string nodeText;

		public TreeNode()
		{
			parent = null;
			children = new TreeNodeList();
		}
		public TreeNode(TreeNode parent)
		{
			this.parent = parent;
			children = new TreeNodeList();
		}
		public String NodeID
		{
			get{return nodeID;}
			set{nodeID = value;}
		}
		public String NodeName
		{
			get{return nodeName;}
			set{nodeName = value;}
		}
		public object NodeData
		{
			get{return nodeData;}
			set{nodeData = value;}
		}
		public string Text
		{
			get{return nodeText;}
			set{nodeText = value;}
		}
		public TreeNodeList Children
		{
			get{return children;}
			set{children = value;}
		}
		public TreeNode Parent
		{
			get{return parent;}
			set{parent = value;}
		}

		/// <summary>
		/// Returns true if this is a leaf node - ie has no children
		/// </summary>
		public bool IsLeaf
		{
			get 
			{
				return (children == null ? true :  false);
			}
		}

		/// <summary>
		/// Returns true if this is a root node - ie has not parent
		/// </summary>
		public bool IsRoot
		{
			get 
			{
				return (parent == null ? true :  false);
			}
		}

		/// <summary>
		/// Same as calling NodeList.SelectedSingleNode([tree_node], nodeID);
		/// </summary>
		public TreeNode SelectedSingleNode(string nodeID) 
		{
			return TreeNodeList.SelectedSingleNode(this, nodeID);
		}


		#region Implementation of ISerializableToNameValueCollection
		/// <summary>
		/// implementation of ISerializableToNameValueCollection.SerializeToNameValueCollection
		/// </summary>
		public void SerializeToNameValueCollection(NameValueCollection col)
		{
			SerializeToNameValueCollection(0,col);
		}
		internal virtual void SerializeToNameValueCollection(int indentLevel, NameValueCollection col)
		{
			string sClassName = this.GetType().ToString();
			string namePrefix = sClassName + ":NodeID[" + this.NodeID + "]";
			namePrefix = namePrefix.PadLeft((indentLevel*2) + namePrefix.Length, '-');

			col.Add(namePrefix + ".IsLeaf", this.IsLeaf.ToString());
			col.Add(namePrefix + ".IsRoot", this.IsRoot.ToString());
			col.Add(namePrefix + ".NodeName", this.NodeName);
			col.Add(namePrefix + ".nodeText", this.nodeText);
			col.Add(namePrefix + ".nodeData", this.nodeData.ToString());
			col.Add(namePrefix + ".# of Children", this.Children == null ? "0" : this.Children.Count.ToString());
			col.Add(namePrefix + ".Parent.NodeID", this.Parent == null ? " -- no parent, this is root node --" : this.Parent.NodeID);

			if (this.Children != null) 
			{
				this.Children.SerializeToNameValueCollection(indentLevel + 1, col);
			}
		}
		#endregion
	}
}