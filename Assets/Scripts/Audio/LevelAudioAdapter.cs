using System;
using UnityEngine;

public class LevelAudioAdapter : MonoBehaviour
{
   void OnEnable()
   {
      ReactionSoundEvent.OnReactionEventInvoke +=PlaySound;
   }

   void OnDisable()
   {
      ReactionSoundEvent.OnReactionEventInvoke -=PlaySound;
   }
   void PlaySound(string clipName)
   {
      AudioController.Instance.PlaySound(clipName);
   }
}
