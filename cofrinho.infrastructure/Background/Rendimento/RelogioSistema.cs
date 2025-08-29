using System;
using cofrinho.core.Abstractions.Services;

namespace cofrinho.infrastructure.Background.Rendimento;

public class RelogioSistema : IRelogio
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateTimeOffset AgoraNoFuso(string fusoHorarioId)
    {
        var tz = ObterTimeZone(fusoHorarioId);
        var utcNow = DateTimeOffset.UtcNow;
        return TimeZoneInfo.ConvertTime(utcNow, tz);
    }

    public TimeZoneInfo ObterTimeZone(string fusoHorarioId)
    {
        // Tenta usar o ID fornecido diretamente
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(fusoHorarioId);
        }
        catch
        {
            // Mapear IANA comuns para Windows quando necessário
            var mapped = MapearIanaParaWindows(fusoHorarioId);
            return TimeZoneInfo.FindSystemTimeZoneById(mapped);
        }
    }

    private static string MapearIanaParaWindows(string id)
    {
        // Mapeamentos mínimos para suportar America/Sao_Paulo em Windows
        return id switch
        {
            "America/Sao_Paulo" => "E. South America Standard Time",
            _ => id
        };
    }
}