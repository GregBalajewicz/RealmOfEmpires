using System;
using System.Collections;
using System.Collections.Specialized;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Collections
{
	/// <summary>
	/// a collection of TreeNodes
	/// </summary>
	public class TreeNodeList : Gmbc.Common.GmbcBaseClass, IEnumerable, ISerializableToNameValueCollection
	{
		ArrayList list;

		public TreeNodeList() 
		{
			list = new ArrayList();
		}

		public virtual int Add(TreeNode value)
		{
			return list.Add (value);
		}

		public virtual TreeNode this[int index]
		{
			get
			{
				return (TreeNode)list[index];
			}
		}
		public int Count
		{
			get 
			{
				return list.Count;
			}
		}

		/// <summary>
		/// Searches the entire tree collection for the first node with the specified nodeID.
		/// Searches all the nodes in this collection and all the children nodes of these nodes
		/// Performs a breath-first search
		/// </summary>
		/// <param name="nodeID"></param>
		/// <returns>Null if no such node is found otherwise it returns the found node.</returns>
		public TreeNode SelectSingleNode(string nodeID)
		{
			return SelectedSingleNode(this, nodeID);
		}

		protected static TreeNode SelectedSingleNode(TreeNodeList nodeList, string nodeID) 
		{
			if (nodeList == null) 
			{
				return null;
			}

			//
			// First search the 1st level 
			foreach(TreeNode node in nodeList) 
			{
				if (node.NodeID == nodeID) 
				{
					return node;
				}
			}
			//
			// Now check the sublevels 
			TreeNode returnNode = null;
			foreach(TreeNode node in nodeList) 
			{
				returnNode = SelectedSingleNode(node.Children,nodeID);
				if (returnNode != null) 
				{
					break;	// found the node! break out of the look and return it
				}
			}

			return returnNode;
		}


		/// <summary>
		/// Searches the entire tree collection, starting with the specified root node and then its children
		/// for the first node with the specified nodeID.
		/// Performs a breath-first search
		/// </summary>
		/// <returns>Null if no such node is found otherwise it returns the found node.</returns>
		public static TreeNode SelectedSingleNode(TreeNode rootNode, string nodeID) 
		{
			if (rootNode.NodeID == nodeID) 
			{
				return rootNode;
			}
			return SelectedSingleNode(rootNode.Children, nodeID);
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
			foreach(TreeNode node in this) 
			{
				node.SerializeToNameValueCollection(indentLevel, col);
			}
		}
		#endregion

		public IEnumerator GetEnumerator() 
		{
			return list.GetEnumerator();
		}
	}
}