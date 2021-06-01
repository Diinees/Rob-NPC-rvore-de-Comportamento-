using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

    //velocidade
	float speed = 20.0F;
    //velocidade de rotacao
    float rotationSpeed = 120.0F;
    //prefab da bala
    public GameObject bulletPrefab;
    //selecionar spawn da bala
    public Transform bulletSpawn;

    void Update() {
        //vertical x velocidade
        float translation = Input.GetAxis("Vertical") * speed;
        //movimento x velocidade
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //translation x delta time
        translation *= Time.deltaTime;
        //rotation x delta time
        rotation *= Time.deltaTime;
        //movimento vertical
        transform.Translate(0, 0, translation);
        //rotacao horizontal
        transform.Rotate(0, rotation, 0);
        
        //atira 
        if(Input.GetKeyDown("space"))
        {
            //instancia bala
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            //adiciona força
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
