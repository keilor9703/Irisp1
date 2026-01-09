using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

public sealed class OracleXmlRepository : IXmlRepository
{
    private readonly string _connString;
    private readonly ILogger<OracleXmlRepository> _logger;

    public OracleXmlRepository(string connString, ILogger<OracleXmlRepository> logger)
    {
        _connString = connString;
        _logger = logger;
    }

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var list = new List<XElement>();

        try
        {
            using var conn = new OracleConnection(_connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText = @"
                SELECT XML_DATA
                FROM IRISP_DP_KEYS
                ORDER BY UPDATED_AT DESC";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var xml = reader.GetString(0);
                if (string.IsNullOrWhiteSpace(xml)) continue;

                // El XML suele tener raíz <key ... />
                var element = XElement.Parse(xml);
                list.Add(element);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leyendo IRISP_DP_KEYS (DataProtection).");
            throw; // Mejor fallar temprano que romper auth silenciosamente
        }

        return list;
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        try
        {
            var xml = element.ToString(SaveOptions.DisableFormatting);

            using var conn = new OracleConnection(_connString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.BindByName = true;

            // MERGE para upsert por friendlyName
            cmd.CommandText = @"
                MERGE INTO IRISP_DP_KEYS t
                USING (SELECT :p_name AS FRIENDLY_NAME, :p_xml AS XML_DATA FROM dual) s
                ON (t.FRIENDLY_NAME = s.FRIENDLY_NAME)
                WHEN MATCHED THEN
                  UPDATE SET t.XML_DATA = s.XML_DATA, t.UPDATED_AT = SYSDATE
                WHEN NOT MATCHED THEN
                  INSERT (FRIENDLY_NAME, XML_DATA, CREATED_AT, UPDATED_AT)
                  VALUES (s.FRIENDLY_NAME, s.XML_DATA, SYSDATE, SYSDATE)";

            cmd.Parameters.Add("p_name", OracleDbType.Varchar2, friendlyName, System.Data.ParameterDirection.Input);
            cmd.Parameters.Add("p_xml", OracleDbType.Clob, xml, System.Data.ParameterDirection.Input);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error guardando DataProtection key {FriendlyName} en IRISP_DP_KEYS.", friendlyName);
            throw;
        }
    }
}
