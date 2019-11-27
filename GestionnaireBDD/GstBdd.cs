using ClassesMetier;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace GestionnaireBDD
{
    public class GstBdd
    {
        MySqlConnection cnx;
        MySqlCommand cmd;
        MySqlDataReader dr;

        public GstBdd()
        {
            string driver = "server=localhost;user id=root;password=;database=gestionsalles";
            cnx = new MySqlConnection(driver);
            cnx.Open();
        }

        public List<Manifestation> GetAllManifestations()
        {
            

            List<Manifestation> lesManifs = new List<Manifestation>();
            cmd = new MySqlCommand("SELECT idManif, nomManif, dateDebut, dateFin, numSalle, nomSalle, nbPlaces FROM manifestation INNER JOIN salle ON numSalle = idSalle", cnx); // requête sql pr récup tarif
            dr = cmd.ExecuteReader();
            while (dr.Read()) // TANT QU'IL Y EN A == TANT QUE VRAI
            {
                Salle uneSalle = new Salle()
                {
                    IdSalle = Convert.ToInt16(dr[4].ToString()),
                    NbPlaces = Convert.ToInt16(dr[6].ToString()),
                    NomSalle = dr[5].ToString()
                };
                Manifestation uneManif = new Manifestation() // Instanciation d'une catégorie
                {
                    IdManif = Convert.ToInt16(dr[0].ToString()),
                    NomManif = dr[1].ToString(),
                    DateDebutManif = dr[2].ToString(),
                    DateFinManif = dr[3].ToString(),
                    LaSalle = uneSalle
                };

                // Ajout de la catégorie a la liste
                lesManifs.Add(uneManif);
            }
            dr.Close();
            return lesManifs;
        }

        public List<Place> GetAllPlacesByIdManifestation(int idManif,int idSalle)
        {
            List<Place> lesPlaces = new List<Place>();

            cmd = new MySqlCommand("SELECT idPlace, t.idTarif, t.prix, o.libre FROM manifestation inner join occuper o on idManif = numManif inner join place p on o.numPlace = p.idPlace INNER JOIN tarif t ON idTarif = numTarif WHERE idManif = " + idManif + " AND p.numSalle = " + idSalle, cnx);

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                bool occupeOuPas = Convert.ToBoolean(dr[3].ToString());
                char etatLibreOuPas;
                if (occupeOuPas == true)
                {
                    etatLibreOuPas = 'o';
                }
                else
                {
                    etatLibreOuPas = 'l';
                }
                Place laPlace = new Place()
                {
                    CodeTarif = Convert.ToChar(dr[1].ToString()),
                    IdPlace = Convert.ToInt16(dr[0].ToString()),
                    Occupee = occupeOuPas,
                    Prix = Convert.ToDouble(dr[2].ToString()),
                    Etat = etatLibreOuPas
                };
                lesPlaces.Add(laPlace);
            }
            dr.Close();

            return lesPlaces;
        }

        public List<Tarif> GetAllTarifs()
        {
            List<Tarif> lesTarifs = new List<Tarif>();
            cmd = new MySqlCommand("SELECT idTarif, nomTarif, prix FROM tarif", cnx); // requête sql pr récup tarif
            dr = cmd.ExecuteReader();
            while (dr.Read()) // TANT QU'IL Y EN A == TANT QUE VRAI
            {
                Tarif unTarif = new Tarif()
                {
                    IdTarif = Convert.ToChar(dr[0]),
                    NomTarif = dr[1].ToString(),
                    Prix = Convert.ToDouble(dr[2].ToString())
                };

                // Ajout de la catégorie a la liste
                lesTarifs.Add(unTarif);
            }
            dr.Close();
            return lesTarifs;
        }

        public void ReserverPlace(int idPlace, int idSalle,int idManif)
        {
            cmd = new MySqlCommand("UPDATE OCCUPER SET libre = 1 WHERE numManif = " + idManif + " AND numSalle = " + idSalle + " AND numPlace = " + idPlace, cnx);
            cmd.ExecuteNonQuery();
        }
    }
}
