open FSharp.Data

type DataTypeField =
    { name: string
      longName: string
      fieldType: string
      position: int
      optional: bool
      }

type DataTypeDefinition =
    { name: string
      fieldCount: int
      fields: DataTypeField list }

type DataTypeSchema = XmlProvider<Sample="schema/datatypes.xsd", ResolutionFolder=__SOURCE_DIRECTORY__>
let dataTypesSchema =
    DataTypeSchema.Load("schema/datatypes.xsd")

let getXmlDataTypeDefinitions () =
    let complexTypes = dataTypesSchema.ComplexTypes

    let typeDefs =
        complexTypes
        |> Array.filter (fun x -> not (x.Name.Contains('.')))

    let typeAttributes =
        dataTypesSchema.AttributeGroups
        |> Array.filter (fun x -> (x.Name.EndsWith(".ATTRIBUTES")))

    let typeContents =
        complexTypes
        |> Array.filter (fun x -> (x.Name.EndsWith(".CONTENT")))

    typeDefs
    |> Array.filter (fun x -> not (x.Name.Contains '.'))
    |> Array.filter
        (fun x ->
            (typeAttributes
             |> Array.exists
                 (fun y ->
                     (y.Name.StartsWith x.Name)
                     && (y.Name.EndsWith ".ATTRIBUTES"))))
    |> Array.filter
        (fun x ->
            (typeContents
             |> Array.exists
                 (fun y ->
                     (y.Name.StartsWith x.Name)
                     && (y.Name.EndsWith ".CONTENT"))))
    |> Array.map
        (fun x ->
            (x,
             typeAttributes
             |> Array.filter
                 (fun y ->
                     (y.Name.StartsWith x.Name)
                     && (y.Name.EndsWith ".ATTRIBUTES"))))
    |> Array.map
        (fun x ->
            (fst x,
             snd x,
             typeContents
             |> Array.filter
                 (fun y ->
                     (y.Name.StartsWith (fst x).Name)
                     && (y.Name.EndsWith ".CONTENT"))))

let xmlDefs = getXmlDataTypeDefinitions()

//let mapXmlDefinition (xmlDef : DataTypeSchema.ComplexType*DataTypeSchema.AttributeGroup2[]*DataTypeSchema.ComplexType[]) : DataTypeDefinition =
//    let (def, attrs, contents) = xmlDef
//    let name = def.Name
//    let fieldCount = match def.Sequence with | None -> 0 | Some x -> x
//    let fields = match def.Sequence with
//                    | None -> []
//                    | Some x -> x.Elements
//                                |> Array.map (fun x ->
//                                                    let fieldName = x.Ref.Value
//                                                    //collate with attrs and shit
//                                                    )
//                                |> Array.toList
//    



printfn $"%A{xmlDefs}"
