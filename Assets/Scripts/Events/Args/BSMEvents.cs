using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMStateEndedEvent { }

public class BSMSendMessageEvent
{
    public string message;

    public BSMSendMessageEvent(string _message)
    {
        message = _message;
    }
}