using System;
using System.Messaging;
using System.Text;
using System.Xml;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Gmbc.Common.Messaging
{
	/// <summary>
	/// This class is used for MSMQ, and it is going to send the message to the queue.
	/// You can either speicify the string message, or System.Messaging.Message as the parameter,
	/// and the second parameter is label.
	/// Author: Gang Zhang
	/// </summary>
	public class MSMQSender
	{
		private MessageQueue outputQueue;

		public MSMQSender()
		{
		}

		public MSMQSender(string outputQueueName)
		{
			outputQueue = new MessageQueue("FormatName:DIRECT=OS:" + outputQueueName);
			((XmlMessageFormatter)outputQueue.Formatter).TargetTypeNames = new string[]{"System.String,mscorlib"};
		}
		public bool IsValidQueue()
		{
			bool canRead = outputQueue.CanRead;
			bool canWrite = outputQueue.CanWrite;
			if(canRead == false || canWrite == false)
				return false;
			else
				return true;
		}
		public void Send(string message, string label)
		{
			System.Messaging.Message requestMessage = new System.Messaging.Message();

			requestMessage.Formatter = new XmlMessageFormatter();

			requestMessage.Body = message;

			try
			{
				outputQueue.Send(requestMessage,label);
			}
			catch(Exception e)
			{
				BaseApplicationException ex = new BaseApplicationException("Error send the message: " + label, e);
				ExceptionManager.Publish(ex);
				throw ex;
			}			
		}
		public void Send(System.Messaging.Message message, string label)
		{
			try
			{
				outputQueue.Send(message,label);
			}
			catch(Exception e)
			{
				BaseApplicationException ex = new BaseApplicationException("Error send the message: " + label, e);
				ExceptionManager.Publish(ex);
				throw ex;
			}
		}
		// This method is created especially for BinaryMessageFormatter 
		public void Send(object message, string label, string formatter)
		{
			System.Messaging.Message requestMessage = new System.Messaging.Message();

			if(formatter.Equals("BinaryMessageFormatter"))
				requestMessage.Formatter = new BinaryMessageFormatter();
			else 
				requestMessage.Formatter = new XmlMessageFormatter();

			requestMessage.Body = message;

			try
			{
				outputQueue.Send(requestMessage,label);
			}
			catch(Exception e)
			{
				BaseApplicationException ex = new BaseApplicationException("Error send the message: " + label, e);
				ExceptionManager.Publish(ex);
				throw ex;
			}					
		}
		public void Close()
		{
			// close the OutputQueue 
			try 
			{
				if (outputQueue != null) 
				{
					outputQueue.Close();
				}
			}
			catch (Exception ex) 
			{
				ExceptionManager.Publish(ex);
				throw ex;
			}
		}
	}
}