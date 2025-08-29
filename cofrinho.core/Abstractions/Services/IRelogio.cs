using System;

namespace cofrinho.core.Abstractions.Services;

public interface IRelogio
{
    DateTimeOffset UtcNow { get; }
    DateTimeOffset AgoraNoFuso(string fusoHorarioId);
    TimeZoneInfo ObterTimeZone(string fusoHorarioId);
}
