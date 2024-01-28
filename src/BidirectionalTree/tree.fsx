// https://stackoverflow.com/questions/77882770/create-bi-directional-tree-in-f
module NaiveAttempt =
  type Parent = {
      Name : string
      Age : int
      Children : Child list }
  and Child = {
      Name : string
      Parent : Parent }    

  let createParent name children = 
    { Name = name
      Age = 42
      Children = children }

  let createChild name parent = 
    { Name = name
      Parent = parent }

  let create name kids =
      let rec makeChild name = createChild name parent
      and makeParent name = createParent name children
      and parent = name |> makeParent 
      and children = kids |> List.map makeChild
      parent

  create "Peter" [ "Sarah"; "Max" ]
  |> printfn "%A"

// https://gist.github.com/Savelenko/d97a7897ee2f7c8a04fae37be4eb3848
module TreeZipper1 =
  ()

// https://stackoverflow.com/questions/74844575/is-it-possible-to-create-a-tree-in-f-where-each-node-also-knows-its-parent
module TreeZipper2 =
  ()


// https://learnyouahaskell.com/zippers

