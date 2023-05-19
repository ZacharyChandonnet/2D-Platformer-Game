using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Auteur : Zachary Chandonnet
/// #synthese_ZACHARY
/// Classe qui gère les les mouvements de mon ennemi
/// Elle permet de faire sauter l'ennemi selon un délai aléatoire entre min et max
/// Lorsque le joueur touche l'ennemi sur la tête, l'ennemi est détruit et le joueur gagne une vie ainsi qu'un bonus
/// Le code pour les réactions du personnage est dans le script personnage
/// </summary>
public class EnnemiZachary : MonoBehaviour
{
    [Header("Ennemi")]
    [SerializeField] private GameObject _ennemi;
    [SerializeField] private float _delaisMinSaut = 3f; // Délai minimum avant de sauter (1 seconde)
    [SerializeField] private float _delaisMaxSaut = 6f; // Délai maximum avant de sauter (6 secondes)

    [Header("Bonus")]
    [SerializeField] private GameObject _bonus;
    [SerializeField] private SOPerso _donneesPerso;

    private int _bonusVie = 2;

    void Start()
    {
        StartCoroutine(SauterAleatoire());
    }


    /// <summary>
    /// #synthese_ZACHARY
    /// Coroutine qui gère le saut de l'ennemi selon un délai aléatoire entre min et max
    /// Elle boucle à l'infini en attendant un certain temps entre chaque saut
    /// On utilise la fonction SautEnnemi pour simuler le saut
    /// </summary>
    /// <returns></returns>
    IEnumerator SauterAleatoire()
    {
        while (true)
        {
            //#synthese_ZACHARY
            // Attendre un certain temps avant de sauter selon un délai aléatoire entre min et max
            float sautDelais = Random.Range(_delaisMinSaut, _delaisMaxSaut);
            // Attendre le délai selon le resultat de sautDelais
            yield return new WaitForSeconds(sautDelais);

            SautEnnemi();
        }
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui simule le saut de l'ennemi
    /// Elle applique une force aléatoire pour simuler le saut
    /// On utilise la fonction AddForce pour appliquer la force
    /// </summary>
    void SautEnnemi()
    {
        // Vérifiez si le rigidbody est présent sur l'ennemi
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return;
        }
        // Appliquez une force aléatoire pour simuler le saut
        Vector2 jumpForce = new Vector2(Random.Range(0f, 1f), Random.Range(0.5f, 1f));
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }


    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui gère la collision entre l'ennemi et le joueur
    /// Si celui-ci touche au corps de l'ennemi, il perd une vie
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && transform.position.y < collision.gameObject.transform.position.y)
        {
            // Si l'ennemi touche le joueur sur le dessus, détruisez l'ennemi 
            _donneesPerso.vie--;
        }
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui gère la collision entre l'ennemi et le joueur
    /// Si celui-ci touche à la tête de l'ennemi, il gagne une vie et un bonus
    /// On détruit l'ennemi et on instancie la potion à la position de l'ennemi décalée vers la gauche
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && transform.position.y < other.gameObject.transform.position.y)
        {
            _donneesPerso.vie += _bonusVie; // On ajoute une vie puisque celui-ci perd une vite au contact de l'ennemi


            Vector3 positionPotion = transform.position; // Récupérer la position de l'ennemi
            positionPotion.x -= 3f; // Décaler la position de la potion de 1 unité vers la gauche

            Instantiate(_bonus, positionPotion, Quaternion.identity); // Instancier la potion à la position de l'ennemi décalée vers la gauche
            Destroy(gameObject);
        }
    }
}
