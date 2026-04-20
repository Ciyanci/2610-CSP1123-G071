using UnityEngine;
using System.Collections.Generic;

    [CreateAssetMenu(menuName="Data/Card")]
public class CardData : ScriptableObject
{
    //to make it show in unity inspector
 [field: SerializeField] public string Name { get; private set; }
 [field: SerializeField] public string Description {get; private set;}
 [field: SerializeField] public int Cost {get; private set;}
 [field: SerializeField] public Sprite Image {get; private set;}
 [field: SerializeField] public List<CardEffect> Effects {get; private set;}


 
}   

