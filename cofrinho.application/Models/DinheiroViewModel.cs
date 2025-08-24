using cofrinho.core.Enums;

namespace cofrinho.application.Models;

public record DinheiroViewModel(decimal Valor, TipoMoedaEnum TipoMoeda);