using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

    //Variavel veolicade
    float speed = 20.0F;
    //Variavel velocidade de rotação
    float rotationSpeed = 120.0F;
    //Variavel que pega prefab bullet
    public GameObject bulletPrefab;
    //Variavel que pega transform do spawn do bullet
    public Transform bulletSpawn;

    void Update() {
        //Variavel que pega eixo vertical para movimentação
        float translation = Input.GetAxis("Vertical") * speed;
        //Variavel que pega eixo horizontal para a rotação
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //Chama variavel do ultimo quadro até atual
        translation *= Time.deltaTime;
        //Chama variavel do ultimo quadro até atual
        rotation *= Time.deltaTime;
        //Movimenta player
        transform.Translate(0, 0, translation);
        //Rotaciona player
        transform.Rotate(0, rotation, 0);

        //Ao apertar tecla espaço é instanciado prefab bullet na scene
        if (Input.GetKeyDown("space"))
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
