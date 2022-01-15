﻿using DashboardBackend.Database.Models;
using Model;
using static Model.Reconciliation;

namespace DashboardBackend.Parsers
{
    /// <summary>
    /// Converts a list of <see cref="AfstemningEntry"/> which is generated by EFCore into a list of <see cref="Reconciliation"/>, which is a model created for the Dashboard system.
    /// </summary>
    public class ReconciliationParser : IDataParser<AfstemningEntry, List<Reconciliation>>
    {
        /// <inheritdoc/>
        /// <returns>A list of reconciliations.</returns>
        public List<Reconciliation> Parse(List<AfstemningEntry> data)
        {
            return (from item in data
                    let date = item.Afstemtdato
                    let name = item.Description
                    let managerName = item.Manager
                    let status = GetReconciliationStatus(item)
                    let srcCount = item.Srcantal
                    let dstCount = item.Dstantal
                    let toolkitId = item.ToolkitId
                    let srcSql = item.SrcSql
                    let dstSql = item.DstSql
                    select new Reconciliation(date, name, status, managerName, srcCount, dstCount, toolkitId, srcSql, dstSql))
                    .ToList();
        }

        /// <summary>
        /// Gets the <see cref="ReconciliationStatus"/> associated with the specified entry.
        /// </summary>
        /// <param name="entry">The entry to get the type of.</param>
        /// <returns>A <see cref="ReconciliationStatus"/> that suits the specified entry.</returns>
        /// <exception cref="ArgumentException"/>
        private ReconciliationStatus GetReconciliationStatus(AfstemningEntry entry)
        {
            return entry.Afstemresultat switch
            {
                "OK" => ReconciliationStatus.Ok,
                "DISABLED" => ReconciliationStatus.Disabled,
                "FAILED" => ReconciliationStatus.Failed,
                "FAIL MISMATCH" => ReconciliationStatus.FailMismatch,
                _ => throw new ArgumentException(nameof(entry) + " is not a known reconciliation result.")
            };
        }
    }
}