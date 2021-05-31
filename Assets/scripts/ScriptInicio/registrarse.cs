using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using MySql.Data.MySqlClient;

public class registrarse : MonoBehaviour {

    public InputField nombrePadreField;
    public InputField apellidoPadreField;
    public InputField cedulaPadreField;
    public InputField celularPadreField;
    public InputField nombreNinioField;
    public InputField cedulaNinioField;
    public InputField edadNinioField;
    public InputField emailField;
    public InputField passwordField;

    public Button registrarseButton;


    public void RegistrarNuevoUsuario()
    {
        if (registrarseButton.interactable = (cedulaPadreField.text.Length == 10 && cedulaNinioField.text.Length == 10 && passwordField.text.Length >= 8))
        {
            string _log = "`usuario` WHERE `cedula_ninio` LIKE '" + cedulaNinioField.text + "'";
            mysql _sql = GameObject.Find("mysql").GetComponent<mysql>();
            MySqlDataReader result = _sql.Select(_log);

            if (result.HasRows)
            {
                Debug.Log("El usuario ya existe");
                result.Close();
            }
            else
            {
                result.Close();
                _log = "`usuario` (`id`, `nombre_padre`, `apellido_padre`, `cedula_padre`, `celular_padre`, `nombre_ninio`, `cedula_ninio`, `edad_ninio`, `email_padre`, `contraseña`) VALUES (NULL, '" + nombrePadreField.text + "', '" + apellidoPadreField.text + "', '" + cedulaPadreField.text + "', '" + celularPadreField.text + "', '" + nombreNinioField.text + "', '" + cedulaNinioField.text + "', '" + edadNinioField.text + "', '" + emailField.text + "', '" + passwordField.text + "')";
                result = _sql.Insert(_log);
                Debug.Log("El usuario ha sido creado");
                result.Close();
                UnityEngine.SceneManagement.SceneManager.LoadScene(3);
            }
        }
    }

    public void VerifyInputs()
    {
        //registrarseButton.interactable = (cedulaPadreField.text.Length == 10 && cedulaNinioField.text.Length == 10 && passwordField.text.Length >= 8);
    }

}