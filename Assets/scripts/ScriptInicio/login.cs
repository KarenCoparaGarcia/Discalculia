using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using MySql.Data.MySqlClient;

public class login : MonoBehaviour {

    public InputField cedulaNinioField;
    public InputField passwordField;

    public void Logear()
    {
        string _log = "`usuario` WHERE `cedula_ninio` LIKE '" + cedulaNinioField.text + "' AND `contraseña` LIKE '" + passwordField.text + "'";

        mysql _sql = GameObject.Find("mysql").GetComponent<mysql>();
        MySqlDataReader result = _sql.Select(_log);

        if (result.HasRows)
        {
            Debug.Log("Login correcto");
            result.Close();
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        }
        else
        {
            Debug.Log("Cedula o contraseña incorrecta");
            result.Close();
        }
    }

}
