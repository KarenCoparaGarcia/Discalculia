using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;

public class mysql : MonoBehaviour {

    public string servidorBaseDatos;
    public string nombreBaseDatos;
    public string usuarioBaseDatos;
    public string contraseñaBaseDatos;

    private string datosConexion;
    private MySqlConnection conn;

    // Use this for initialization
    void Start () {
        datosConexion = "Server=" + servidorBaseDatos
                        + ";Database=" + nombreBaseDatos
                        + ";Uid=" + usuarioBaseDatos
                        + ";Pwd=" + contraseñaBaseDatos
                        + ";";

        Mysql();
	}

    void Mysql()
    {
        conn = new MySqlConnection(datosConexion);

        try
        {
            conn.Open();
            Debug.Log("Conexion establecida");
        }
        catch (MySqlException error)
        {
            Debug.Log("Conexion fallida: " + error);
        }

    }

    public MySqlDataReader Select(string _select)
    {
        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM " + _select;
        MySqlDataReader result = cmd.ExecuteReader();
        return result;
    }

    public MySqlDataReader Insert(string _insert)
    {
        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO " + _insert;
        MySqlDataReader result = cmd.ExecuteReader();
        return result;
    }
}
