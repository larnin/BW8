using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestFailActionStartObjective : QuestFailActionBase
{
    QuestFailActionObjectStartObjective m_failAction;

    public QuestFailActionStartObjective(QuestFailActionObjectStartObjective failAction)
    {
        m_failAction = failAction;
    }

    public override void Execute()
    {
        //todo
    }
}