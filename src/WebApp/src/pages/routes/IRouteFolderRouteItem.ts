interface IRouteFolderItem {
  type: "Route" | "Folder";
  id: number;
  parentId: number | null;
  route: IRouteItem | null;
  folder: IFolderItem | null;
}

interface IRouteItem {
  name: string;
  method: string;
  path: string;
  status: string;
}

interface IFolderItem {
  name: string;
}
