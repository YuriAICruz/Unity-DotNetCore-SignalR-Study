using System;
using UnityEditor.Build.Content;
using UnityEngine.SceneManagement;

namespace Presentation.Managers
{
    public class CharacterSelectionManager
    {
        public event Action<int> OnCharacterSelected; 
        private int _selectedCharacter = 0;

        public int SelectedCharacter => _selectedCharacter;
        
        public CharacterSelectionManager()
        {
            
        }

        public void SelectCharacter(int index)
        {
            _selectedCharacter = index;
            OnCharacterSelected?.Invoke(_selectedCharacter);
        }

        public void Continue()
        {
            SceneManager.LoadScene(2);
        }
    }
}