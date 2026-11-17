using System.Text.Json;
using System.Text.RegularExpressions;
using Sace.Api.Domain;

namespace Sace.Api.Services;

public sealed record InvoiceCovePair(
  OperationDocument? Invoice,
  OperationDocument? Cove,
  InvoiceCovePairingStatus Status,
  InvoiceCovePairingMethod Method,
  string ReasonCode,
  string Explanation);

public interface IInvoiceCovePairingService {
  IReadOnlyList<InvoiceCovePair> Pair(
    IReadOnlyList<OperationDocument> invoices,
    IReadOnlyList<OperationDocument> coves,
    IReadOnlyList<DocumentRelationshipCandidate> relationships);
}

public sealed class InvoiceCovePairingService : IInvoiceCovePairingService {
  public IReadOnlyList<InvoiceCovePair> Pair(IReadOnlyList<OperationDocument> invoices,IReadOnlyList<OperationDocument> coves,IReadOnlyList<DocumentRelationshipCandidate> relationships){
    if(invoices.Count==1&&coves.Count==1){
      var evidence=Evidence(invoices[0],coves[0],relationships);
      return evidence is null
        ? [new(invoices[0],coves[0],InvoiceCovePairingStatus.MANUAL_REVIEW,InvoiceCovePairingMethod.SINGLE_CANDIDATE,"INSUFFICIENT_EVIDENCE","Existe una sola pareja candidata, pero no evidencia documental que confirme la relación.")]
        : [new(invoices[0],coves[0],InvoiceCovePairingStatus.CONFIRMED,evidence.Value.Method,"PAIR_CONFIRMED",evidence.Value.Explanation)];
    }

    var result=new List<InvoiceCovePair>();
    var usedCoves=new HashSet<Guid>();
    foreach(var invoice in invoices.OrderBy(x=>x.Id)){
      var candidates=coves.Where(x=>!usedCoves.Contains(x.Id))
        .Select(x=>(Cove:x,Evidence:Evidence(invoice,x,relationships)))
        .Where(x=>x.Evidence is not null)
        .ToList();
      if(candidates.Count==1){
        var candidate=candidates[0];usedCoves.Add(candidate.Cove.Id);
        result.Add(new(invoice,candidate.Cove,InvoiceCovePairingStatus.CONFIRMED,candidate.Evidence!.Value.Method,"PAIR_CONFIRMED",candidate.Evidence.Value.Explanation));
      }else if(candidates.Count>1){
        result.Add(new(invoice,null,InvoiceCovePairingStatus.MANUAL_REVIEW,InvoiceCovePairingMethod.NONE,"AMBIGUOUS_PAIRING","Más de un COVE presenta evidencia explícita para la misma factura."));
      }else{
        result.Add(new(invoice,null,InvoiceCovePairingStatus.MANUAL_REVIEW,InvoiceCovePairingMethod.NONE,"INSUFFICIENT_EVIDENCE","No se localizó un COVE con evidencia explícita de relación."));
      }
    }
    foreach(var cove in coves.Where(x=>!usedCoves.Contains(x.Id)).OrderBy(x=>x.Id))
      result.Add(new(null,cove,InvoiceCovePairingStatus.MANUAL_REVIEW,InvoiceCovePairingMethod.NONE,"INSUFFICIENT_EVIDENCE","COVE sin factura relacionada mediante evidencia explícita."));
    return result;
  }

  private static (InvoiceCovePairingMethod Method,string Explanation)? Evidence(OperationDocument invoice,OperationDocument cove,IReadOnlyList<DocumentRelationshipCandidate> relationships){
    var confirmed=relationships.Any(x=>x.ReviewStatus==RelationshipReviewStatus.CONFIRMED&&((x.SourceDocumentId==invoice.Id&&x.TargetDocumentId==cove.Id)||(x.SourceDocumentId==cove.Id&&x.TargetDocumentId==invoice.Id)));
    if(confirmed)return(InvoiceCovePairingMethod.MANUAL_CONFIRMATION,"Pareja confirmada mediante relación documental auditada.");

    var invoiceNumbers=Values(invoice,DocumentFieldCodes.InvoiceNumber).ToHashSet(StringComparer.OrdinalIgnoreCase);
    var coveInvoiceNumbers=Values(cove,DocumentFieldCodes.InvoiceNumber).ToHashSet(StringComparer.OrdinalIgnoreCase);
    if(invoiceNumbers.Count>0&&invoiceNumbers.Overlaps(coveInvoiceNumbers))return(InvoiceCovePairingMethod.INVOICE_NUMBER_EXACT,"Número de factura normalizado coincidente en ambos documentos.");

    var invoiceRefs=invoice.References.Where(IsInvoiceReference).Select(x=>Normalize(x.Value)).Where(x=>x is not null).Select(x=>x!).ToHashSet(StringComparer.OrdinalIgnoreCase);
    var coveRefs=cove.References.Where(IsInvoiceReference).Select(x=>Normalize(x.Value)).Where(x=>x is not null).Select(x=>x!).ToHashSet(StringComparer.OrdinalIgnoreCase);
    if(invoiceRefs.Count>0&&invoiceRefs.Overlaps(coveRefs))return(InvoiceCovePairingMethod.DOCUMENT_REFERENCE,"DocumentReference de factura coincidente en ambos documentos.");
    if(invoiceNumbers.Count>0&&(invoiceNumbers.Overlaps(coveRefs)||invoiceNumbers.Overlaps(Values(cove,DocumentFieldCodes.DocumentReference))))return(InvoiceCovePairingMethod.EXPLICIT_REFERENCE,"Referencia explícita del COVE coincide con el número de factura.");
    return null;
  }

  private static IEnumerable<string> Values(OperationDocument doc,string code){
    var values=doc.Fields.Where(x=>string.Equals(x.FieldCode,code,StringComparison.OrdinalIgnoreCase)||string.Equals(x.CanonicalFieldCode,code,StringComparison.OrdinalIgnoreCase)).Select(x=>Normalize(x.NormalizedValue??x.RawValue));
    if(code==DocumentFieldCodes.InvoiceNumber)values=values.Append(Normalize(doc.DocumentNumber));
    return values.Where(x=>x is not null).Select(x=>x!);
  }
  private static bool IsInvoiceReference(DocumentReference value)=>value.ReferenceType.Contains("INVOICE",StringComparison.OrdinalIgnoreCase)||value.ReferenceType.Contains("FACTURA",StringComparison.OrdinalIgnoreCase)||value.ReferenceType.Equals("DOCUMENT",StringComparison.OrdinalIgnoreCase);
  internal static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:Regex.Replace(value.Trim().ToUpperInvariant(),@"[^A-Z0-9]","");
}

public interface IInvoiceCoveLineMatcher {
  IReadOnlyList<DocumentLineItemMatch> Match(IReadOnlyList<DocumentLineItem> invoiceItems,IReadOnlyList<DocumentLineItem> coveItems);
}

public sealed class InvoiceCoveLineMatcher : IInvoiceCoveLineMatcher {
  public IReadOnlyList<DocumentLineItemMatch> Match(IReadOnlyList<DocumentLineItem> invoiceItems,IReadOnlyList<DocumentLineItem> coveItems){
    var result=new List<DocumentLineItemMatch>();var used=new HashSet<Guid>();
    foreach(var left in invoiceItems.OrderBy(x=>x.Id)){
      var available=coveItems.Where(x=>!used.Contains(x.Id)).ToList();
      var match=Find(left,available);
      if(match.Candidates.Count==1){var right=match.Candidates[0];used.Add(right.Id);result.Add(Row(left,right,match.Method,match.Key,LineItemMatchStatus.MATCHED,$"Partidas vinculadas por identificador explícito {match.Method}: {match.Key}."));}
      else if(match.Candidates.Count>1)result.Add(Row(left,null,match.Method,match.Key,LineItemMatchStatus.AMBIGUOUS,"Más de una partida comparte los mismos identificadores explícitos; no se eligió por posición."));
      else if(match.Key is not null)result.Add(Row(left,null,match.Method,match.Key,LineItemMatchStatus.UNMATCHED_LEFT,"La partida identificada de la factura no existe en el COVE."));
      else result.Add(Row(left,null,LineItemMatchMethod.NONE,null,LineItemMatchStatus.NOT_COMPARABLE,"No existe identificador explícito suficiente para emparejar la partida."));
    }
    foreach(var right in coveItems.Where(x=>!used.Contains(x.Id)).OrderBy(x=>x.Id))result.Add(Row(null,right,LineItemMatchMethod.NONE,null,LineItemMatchStatus.UNMATCHED_RIGHT,"Partida COVE sin pareja determinística."));
    return result;
  }

  private static (LineItemMatchMethod Method,string? Key,List<DocumentLineItem> Candidates) Find(DocumentLineItem left,List<DocumentLineItem> available){
    (LineItemMatchMethod Method,string Key)? explicitIdentifier=null;
    foreach(var candidate in new[]{(LineItemMatchMethod.PART_NUMBER,Key:left.PartNumber,Get:(Func<DocumentLineItem,string?>)(x=>x.PartNumber)),(LineItemMatchMethod.PART_NUMBER,Key:left.Sku,Get:(Func<DocumentLineItem,string?>)(x=>x.Sku)),(LineItemMatchMethod.ITEM_REFERENCE,Key:left.ItemNumber,Get:(Func<DocumentLineItem,string?>)(x=>x.ItemNumber))}){
      var key=InvoiceCovePairingService.Normalize(candidate.Key);if(key is null)continue;
      explicitIdentifier??=(candidate.Item1,key);
      var rows=available.Where(x=>InvoiceCovePairingService.Normalize(candidate.Get(x))==key).ToList();
      if(rows.Count==1)return(candidate.Item1,key,rows);
      if(rows.Count>1){var refined=Refine(left,rows);return(candidate.Item1,key,refined.Count>0?refined:rows);}
    }
    return explicitIdentifier is null?(LineItemMatchMethod.NONE,null,[]):(explicitIdentifier.Value.Method,explicitIdentifier.Value.Key,[]);
  }
  private static List<DocumentLineItem> Refine(DocumentLineItem left,List<DocumentLineItem> rows){
    var identifiers=new[]{(left.PartNumber,(Func<DocumentLineItem,string?>)(x=>x.PartNumber)),(left.Sku,(Func<DocumentLineItem,string?>)(x=>x.Sku)),(left.ItemNumber,(Func<DocumentLineItem,string?>)(x=>x.ItemNumber)),(left.SerialNumber,(Func<DocumentLineItem,string?>)(x=>x.SerialNumber)),(left.Model,(Func<DocumentLineItem,string?>)(x=>x.Model))};
    var current=rows;foreach(var id in identifiers){var key=InvoiceCovePairingService.Normalize(id.Item1);if(key is null)continue;var filtered=current.Where(x=>InvoiceCovePairingService.Normalize(id.Item2(x))==key).ToList();if(filtered.Count>0)current=filtered;if(current.Count==1)break;}return current;
  }
  private static DocumentLineItemMatch Row(DocumentLineItem? left,DocumentLineItem? right,LineItemMatchMethod method,string? key,LineItemMatchStatus status,string explanation)=>new(){LeftLineItemId=left?.Id,RightLineItemId=right?.Id,MatchMethod=method,MatchKey=key,Status=status,Explanation=explanation,LeftItemSnapshot=left is null?null:JsonSerializer.Serialize(Snapshot(left)),RightItemSnapshot=right is null?null:JsonSerializer.Serialize(Snapshot(right))};
  private static object Snapshot(DocumentLineItem x)=>new{x.Sequence,x.ItemNumber,x.PartNumber,x.Sku,x.Description,x.Brand,x.Model,x.Submodel,x.SerialNumber,x.Quantity,x.Unit,x.UnitValue,x.TotalValue,x.Currency,x.RawLocator,x.PageNumber,x.ExtractionMethod};
}
