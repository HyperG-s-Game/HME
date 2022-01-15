using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace WolfGamer.Hold_My_Eggs{
    public class StationeyBowls : Bowls {

        
        protected override void Awake(){
            base.Awake();
        }
        protected override void Update(){
            base.Update();
        }
        public override void OnOffTriggerCollider(bool _on){
            base.OnOffTriggerCollider(_on);
        }
       


    }
}