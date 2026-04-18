using UnityEngine;

    [CreateAssetMenu(menuName="Data/Card")]
public class CardData : ScriptableObject
{
    //to make it show in unity inspector
 [field: SerializeField] public string Description {get; private set;}
 [field: SerializeField] public int Cost {get; private set;}
 [field: SerializeField] public Sprite Image {get; private set;}
 
}   

