﻿using DockScripter.Domain.Entities;

namespace DockScripter.Services.Interfaces;

public interface IExecutionService
{
    Task<ExecutionResultEntity> ExecuteScriptInContainerAsync(ScriptEntity script, CancellationToken cancellationToken);
    Task<ExecutionResultEntity> GetResultsByScriptId(Guid scriptId, CancellationToken cancellationToken);
}