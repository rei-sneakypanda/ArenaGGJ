using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerOneScoreText;
    [SerializeField] private TMP_Text _playerTwoScoreText;

    private int _playerOneScore;
    private int _playerTwoScore;

    private void Update()
    {
        if (PlayersManager.Instance.PlayerOne == null)
        {
            return;
        }
        
        if (_playerOneScore < PlayersManager.Instance.PlayerOne.PlayerScore.Value)
        {
            _playerOneScore++;
        }
         
        if (_playerTwoScore < PlayersManager.Instance.PlayerTwo.PlayerScore.Value)
        {
            _playerTwoScore++;
        }
        
        _playerOneScoreText.text = _playerOneScore.ToString();
        _playerTwoScoreText.text = _playerTwoScore.ToString();
    }
}
