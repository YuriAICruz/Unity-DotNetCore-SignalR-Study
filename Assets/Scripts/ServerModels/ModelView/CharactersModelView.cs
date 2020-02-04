using System;
using System.ComponentModel.DataAnnotations;
#if !UNITY_2018_3_OR_NEWER
using WebServerStudy.Models;
#endif

#if UNITY_2018_3_OR_NEWER
using UnityEngine;
#endif

namespace Graphene.SharedModels.ModelView
{
    [Serializable]
    public class CharactersModelView
    {
        [Required] public int Id;
        [Required] public string Name;

        public float[] Color;

        public CharactersModelView()
        {
        }
        
#if !UNITY_2018_3_OR_NEWER
        public CharactersModelView(Character character)
        {
            Id = character.Id;
            Name = character.Name;
            Color = new []{character.ColorR, character.ColorG, character.ColorB, character.ColorA} ;
        }
#endif


#if UNITY_2018_3_OR_NEWER
        public Color GetColor()
        {
            return new Color(Color[0], Color[1], Color[2], Color[3]);
        }
#endif
    }
}