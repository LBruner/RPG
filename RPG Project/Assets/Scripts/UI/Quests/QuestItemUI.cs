using System;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] TextMeshProUGUI completion;
        Quest quest;

        public void Setup(Quest quest)
        {
            text.text = quest.GetTitle();
            completion.text = "0/" + quest.GetObjectivesNum();
            this.quest = quest;
        }

        public Quest GetQuest()
        {
            return quest;
        }
    }
}