using UPB.CoreLogic.Models;
//using Serilog;
using System;
namespace UPB.CoreLogic.Managers;

public class PatientManager
{
    private string _path;
    
    public PatientManager(string path)
    {
        _path = path;
    }

    public List<Patients> GetAll()
    {
        List<Patients> patients = new List<Patients>();
        using (StreamReader reader = new StreamReader(_path))
        {
            string line;
            string[] datos;
            Patients newPatient;
            while(!reader.EndOfStream)
            {
                line = reader.ReadLine();
                datos = line.Split(',');
                newPatient = new Patients() 
                {
                    Name = datos[0],
                    LastName = datos[1],
                    BloodGroup = datos[2],
                    Age = int.Parse(datos[3]),
                    CI = int.Parse(datos[4])
                };
                patients.Add(newPatient);
            }   
        }
        return patients;
    }

    public Patients GetByCI(int ci)
    {
        if (ci < 0)
        {
            throw new Exception("CI invalido");
        }

        Patients patientfound = null;
        using (StreamReader reader = new StreamReader(_path))
        {
            string line;
            string[] datos;
            while(!reader.EndOfStream)
            {
                line = reader.ReadLine();
                datos = line.Split(',');
                if(int.Parse(datos[4]) == ci)
                {
                    patientfound = new Patients() 
                    {
                        Name = datos[0],
                        LastName = datos[1],
                        BloodGroup = datos[2],
                        Age = int.Parse(datos[3]),
                        CI = int.Parse(datos[4])
                    };
                }
            }
        }
        if(patientfound == null)
        {
            throw new Exception("Error, Patient not found");
        }
        return patientfound;
    }

    public Patients Update(int ci, string name, string lastname)
    {
        if (ci < 0)
        {
            throw new Exception("CI invalido");
        }

        Patients patientfound = GetByCI(ci);
        int indexLine = findIndex(ci);

        List<string> lines = File.ReadAllLines(_path).ToList();

        if (lines.Count >= indexLine)
        {
            lines[indexLine] = string.Format("{0},{1},{2},{3},{4}",name,lastname,patientfound.BloodGroup,patientfound.Age.ToString(),ci.ToString());
            patientfound.Name = name;
            patientfound.LastName =  lastname;
        }

        using (StreamWriter writer = new StreamWriter(_path))
        {
            foreach (string linea in lines)
            {
                writer.WriteLine(linea);
            }
        }

        return patientfound;
    }
    public Patients Create(string name, string lastname, int age, int ci)
    {
        if(ci < 0)
        {
            throw new Exception("Error, CI no valido");
        }

        using(StreamReader reader = new StreamReader(_path))
        {
            string line;
            string[] datos;
            while(!reader.EndOfStream)
            {
                Console.WriteLine("ENtro");
                line = reader.ReadLine();
                datos = line.Split(',');
                if(datos != null && int.Parse(datos[4]) == ci)
                {
                    throw new Exception("Error, CI existente");
                }
            }
        }

        Random rnd = new Random();
        string[] BloodGroup = new string[] {"A+","A-","B+","B-","AB+","AB-","O-","O+"};
        string bloodGroupSelected = BloodGroup[rnd.Next(0,7)];
        Patients createdPatient = new Patients()
        {
            Name = name,
            LastName = lastname,
            Age = age,
            CI = ci,
            BloodGroup = bloodGroupSelected
        };

        using (FileStream fs = new FileStream(_path, FileMode.Append))
        using(StreamWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine("{0},{1},{2},{3},{4}",name,lastname,bloodGroupSelected,age.ToString(),ci.ToString());
        }
        return createdPatient;
    }

    public Patients Delete(int ci)
    {
        Patients patientToDelete = GetByCI(ci);
        int indexLine = findIndex(ci);

        if (patientToDelete == null)
        {
            throw new Exception("Error, Patient not found");
        }

        // Leer todas las líneas del archivo
        List<string> lines = File.ReadAllLines(_path).ToList();

        if (lines.Count >= indexLine)
        {
            lines.RemoveAt(indexLine);
        }

        using (StreamWriter writer = new StreamWriter(_path))
        {
            foreach (string linea in lines)
            {
                writer.WriteLine(linea);
            }
        }

        return patientToDelete;
    }

    private int findIndex(int ci)
    {
        int indexLine = 0;
        using (StreamReader reader = new StreamReader(_path))
        {
            string line;
            string[] datos;
            while(!reader.EndOfStream)
            {
                line = reader.ReadLine();
                datos = line.Split(',');
                if(int.Parse(datos[4]) == ci)
                {
                    break;
                }
                indexLine++;
            }
        }
        return indexLine;
    }
}