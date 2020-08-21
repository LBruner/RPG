﻿using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue = null;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        public event Action conversationUpdated;

        public void StartDialogue(AIConversant conversant, Dialogue dialogue)
        {
            currentDialogue = dialogue;
            currentConversant = conversant;
            Next();
        }

        public void EndDialogue()
        {
            currentDialogue = null;
            SetCurrentNode(null);
            currentConversant = null;
            isChoosing = false;
            if (conversationUpdated != null)
            {
                conversationUpdated();
            }
        }

        public bool HasActiveConversation()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetCurrentText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetCurrentChoices()
        {
            foreach (DialogueNode child in currentDialogue.GetChildren(currentNode))
            {
                if (CheckConditions(child.GetConditions()))
                {
                    yield return child;
                }
            }
        }

        public void Next()
        {
            if (currentDialogue.IsPlayerNext(currentNode))
            {
                isChoosing = true;
                if (conversationUpdated != null)
                {
                    conversationUpdated();
                }
                return;
            }

            List<DialogueNode> choices = new List<DialogueNode>(GetCurrentChoices());
            int choice = UnityEngine.Random.Range(0, choices.Count);
            SetCurrentNode(choices[choice]);
            if (conversationUpdated != null)
            {
                conversationUpdated();
            }
        }

        public bool HasNext()
        {
            if (currentDialogue == null) return false;
            if (isChoosing) return false;

            List<DialogueNode> choices = new List<DialogueNode>(currentDialogue.GetChildren(currentNode));
            return choices.Count > 0;
        }

        public void ChooseNext(DialogueNode choice)
        {
            SetCurrentNode(choice);
            isChoosing = false;
            Next();
        }

        void SetCurrentNode(DialogueNode newNode)
        {
            if (currentNode != null)
            {
                TriggerEvents(currentNode.GetOnExitTriggers());
            }
            currentNode = newNode;
            if (currentNode != null)
            {
                TriggerEvents(currentNode.GetOnEnterTriggers());
            }
        }

        void TriggerEvents(string[] triggers)
        {
            foreach (DialogueTrigger triggerScript in currentConversant.GetComponents<DialogueTrigger>())
            {
                foreach (string trigger in triggers)
                {
                    triggerScript.Trigger(trigger);
                }
            }
        }

        bool CheckConditions(string[] conditions)
        {
            foreach (string condition in conditions)
            {
                if (!CheckCondition(condition)) return false;
            }

            return true;
        }

        bool CheckCondition(string condition)
        {
            foreach (IConditionEvaluator evaluator in currentConversant.GetComponents<IConditionEvaluator>())
            {
                if (evaluator.Evaluate(condition)) return true;
            }
            foreach (IConditionEvaluator evaluator in GetComponents<IConditionEvaluator>())
            {
                if (evaluator.Evaluate(condition)) return true;
            }

            return false;
        }

    }
}
