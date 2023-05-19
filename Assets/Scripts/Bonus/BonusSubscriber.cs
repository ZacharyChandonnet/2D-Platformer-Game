using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #Synthèse_ALBERT
/// Classe qui permet d'activer le bonus avec l'activateur 
/// Auteur du code: Albert Jannard
/// Commentaire: Albert Jannard
/// </summary>
public class BonusSubscriber : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private SOPerso _donnees;

    [Header("Composants")]
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    private void OnEnable()
    {
        _donnees.bonusActivated.AddListener(ActivateBonus);
    }

    private void OnDisable()
    {
        _donnees.bonusActivated.RemoveListener(ActivateBonus);
    }

    private void ActivateBonus()
    {
        _spriteRenderer.color = Color.white;
        _collider.enabled = true;
        Debug.Log("Bonus activated");
    }

}

