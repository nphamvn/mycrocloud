import { Outlet, useNavigate, useParams, useMatch } from "react-router-dom";
import { useContext, useEffect, useReducer, useRef, useState } from "react";
import IRoute from "./Route";
import { AppContext } from "../apps";
import { useAuth0 } from "@auth0/auth0-react";
import {
  RoutesContext,
  routesReducer,
  //useRoutesContext,
} from "./RoutesContext";
import { EllipsisVerticalIcon } from "@heroicons/react/24/solid";
//import { toast } from "react-toastify";

interface Node {
  type: string;
  id: number;
  parentId: number | null;
  route: IRoute | null;
  folder: {
    name: string;
  } | null;
}

export default function RouteIndex() {
  const app = useContext(AppContext)!;
  const [state, dispatch] = useReducer(routesReducer, {
    routes: [],
    activeRoute: undefined,
  });
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const params = useParams();
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;
  useEffect(() => {
    const getRoutes = async () => {
      const accessToken = await getAccessTokenSilently();
      const routes = (await (
        await fetch(`/api/apps/${app.id}/routes`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as IRoute[];
      dispatch({ type: "SET_ROUTES", payload: routes });
    };
    getRoutes();
  }, []);

  const handleNewFolderClick = async () => {
    const folderName = prompt("Enter folder name");
    if (folderName) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/routes/folders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({ name: folderName }),
      });
      if (res.ok) {
        console.log("Folder created");
      } else {
        console.log("Folder creation failed");
      }
    }
  };
  const newRouteActive = useMatch("/apps/:appId/routes/new/:folderId?");
  const editRouteActive = useMatch("/apps/:appId/routes/:routeId");
  const logPageActive = useMatch("/apps/:appId/routes/:routeId/logs");

  return (
    <RoutesContext.Provider value={{ state, dispatch }}>
      <div className="flex h-full">
        <div className="w-48 border-r p-1">
          <div className="flex justify-center space-x-2">
            <button
              type="button"
              onClick={() => {
                navigate("new");
              }}
              className="bg-primary px-2 py-1 text-white enabled:hover:bg-cyan-700 disabled:opacity-50"
            >
              New Route
            </button>
            <button
              type="button"
              onClick={handleNewFolderClick}
              className="bg-primary px-2 py-1 text-white enabled:hover:bg-cyan-700 disabled:opacity-50"
            >
              New Folder
            </button>
          </div>

          <hr className="my-1" />
          <TreeNode />
        </div>
        <div className="flex-1">
          {newRouteActive || editRouteActive || logPageActive ? (
            <div className="">
              <Outlet key={routeId} />
            </div>
          ) : (
            <div className="flex h-screen items-center justify-center">
              Click New button to create new route or click route to edit.
            </div>
          )}
        </div>
      </div>
    </RoutesContext.Provider>
  );
}

function TreeNode() {
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();

  const app = useContext(AppContext)!;
  const [nodes, setNodes] = useState<Node[]>([]);

  useEffect(() => {
    const getRoutes = async () => {
      const accessToken = await getAccessTokenSilently();
      fetch(`/api/apps/${app.id}/routes/v2`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
        .then((res) => res.json())
        .then((routes) => {
          console.log(routes);
          setNodes(routes);
        });
    };
    getRoutes();
  }, []);

  const [actionMenuRoute, setActionMenuRoute] = useState<Node>();

  const actionMenuRef = useRef<HTMLDivElement>(null);
  const actionMenuClientXRef = useRef<number>();
  const actionMenuClientYRef = useRef<number>();

  function calculateActionMenuTop() {
    if (!actionMenuRef.current || !actionMenuClientYRef.current) {
      return -9999;
    }
    return actionMenuClientYRef.current - actionMenuRef.current.offsetTop;
  }
  function calculateActionMenuRight() {
    if (!actionMenuClientXRef.current) {
      return -9999;
    }
    return 20;
  }

  const handleActionMenuClick = (
    route: Node,
    clientX: number,
    clientY: number,
  ) => {
    actionMenuClientXRef.current = clientX;
    actionMenuClientYRef.current = clientY;
    setActionMenuRoute(route);
  };

  const handleNewRouteClick = async () => {
    navigate(`new/${actionMenuRoute!.id}`);

    setNodes((nodes) => {
      const newRoute: Node = {
        type: "Route",
        id: 0,
        parentId: actionMenuRoute!.id,
        route: {
          id: 0,
          name: "New Route",
          method: "GET",
          path: "/new-route",
          responseType: "static",
          status: "Active",
          requireAuthorization: false,
          useDynamicResponse: false,
        },
        folder: null,
      };
      return [...nodes, newRoute];
    });
  };

  const handleNewFolderClick = async () => {
    const folderName = prompt("Enter folder name");
    if (folderName) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/routes/folders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({
          name: folderName,
          parentId: actionMenuRoute!.id,
        }),
      });
      if (res.ok) {
        setNodes((nodes) => {
          const newFolder: Node = {
            type: "Folder",
            id: parseInt(res.headers.get("Location")!),
            parentId: actionMenuRoute!.id,
            route: null,
            folder: {
              name: folderName,
            },
          };
          return [...nodes, newFolder];
        });
      } else {
        console.log("Folder creation failed");
      }
    }
  };

  const handleDuplicateClick = async () => {
    // const accessToken = await getAccessTokenSilently();
    // const res = await fetch(
    //   `/api/apps/${app.id}/routes/${actionMenuRoute!.id}/clone`,
    //   {
    //     method: "POST",
    //     headers: {
    //       Authorization: `Bearer ${accessToken}`,
    //     },
    //   },
    // );
    // const newRoute = (await res.json()) as IRoute;
    // if (res.ok) {
    //   toast.success("Route cloned");
    //   dispatch({ type: "ADD_ROUTE", payload: newRoute });
    // }
  };
  const handleDeleteClick = async () => {
    if (confirm("Are you sure want to delete this route?")) {
      // const accessToken = await getAccessTokenSilently();
      // const res = await fetch(
      //   `/api/apps/${app.id}/routes/${actionMenuRoute!.id}`,
      //   {
      //     method: "DELETE",
      //     headers: {
      //       Authorization: `Bearer ${accessToken}`,
      //     },
      //   },
      // );
      // if (res.ok) {
      //   dispatch({
      //     type: "DELETE_ROUTE",
      //     payload: routes.find((r) => r.id === actionMenuRoute!.id)!,
      //   });
      //   if (actionMenuRoute!.id === activeRoute?.id) {
      //     navigate("./");
      //   }
      // }
    }
  };

  const renderTree = (
    nodes: Node[],
    parentId: number | null,
    isRoot: boolean = false,
  ) => {
    return (
      <>
        {nodes
          .filter((node) => node.parentId === parentId)
          .map((node) => {
            console.log("rendering node", node);
            return (
              <div key={node.id} style={{ marginLeft: isRoot ? 0 : 20 }}>
                {node.folder ? (
                  <FolderItem
                    folder={node.folder}
                    onActionMenuClick={(buttonLeft, buttonTop) =>
                      handleActionMenuClick(node, buttonLeft, buttonTop)
                    }
                  />
                ) : (
                  <RouteItem
                    route={node.route}
                    id={node.id}
                    onActionMenuClick={(buttonLeft, buttonTop) =>
                      handleActionMenuClick(node, buttonLeft, buttonTop)
                    }
                  />
                )}
                {renderTree(nodes, node.id)}
              </div>
            );
          })}
      </>
    );
  };

  return (
    <div>
      <div ref={actionMenuRef} className="relative">
        {actionMenuRoute && (
          <ActionMenu
            top={calculateActionMenuTop()}
            right={calculateActionMenuRight()}
            node={actionMenuRoute}
            onNewRouteClick={handleNewRouteClick}
            onNewFolderClick={handleNewFolderClick}
            onDuplicateClick={handleDuplicateClick}
            onDeleteClick={handleDeleteClick}
            onOutSideClick={() => setActionMenuRoute(undefined)}
          />
        )}
      </div>
      {renderTree(nodes, null, true)}
    </div>
  );
}

// function RouteList() {
//   const app = useContext(AppContext)!;
//   const { getAccessTokenSilently } = useAuth0();
//   const {
//     state: { routes, activeRoute },
//     dispatch,
//   } = useRoutesContext();

//   const navigate = useNavigate();
//   const [actionMenuRoute, setActionMenuRoute] = useState<IRoute>();

//   const actionMenuRef = useRef<HTMLDivElement>(null);
//   const actionMenuClientXRef = useRef<number>();
//   const actionMenuClientYRef = useRef<number>();

//   function calculateActionMenuTop() {
//     if (!actionMenuRef.current || !actionMenuClientYRef.current) {
//       return -9999;
//     }
//     return actionMenuClientYRef.current - actionMenuRef.current.offsetTop;
//   }
//   function calculateActionMenuRight() {
//     if (!actionMenuClientXRef.current) {
//       return -9999;
//     }
//     return 20;
//   }

//   const handleActionMenuClick = (
//     route: IRoute,
//     clientX: number,
//     clientY: number,
//   ) => {
//     actionMenuClientXRef.current = clientX;
//     actionMenuClientYRef.current = clientY;
//     setActionMenuRoute(route);
//   };
//   const handleDuplicateClick = async () => {
//     const accessToken = await getAccessTokenSilently();
//     const res = await fetch(
//       `/api/apps/${app.id}/routes/${actionMenuRoute!.id}/clone`,
//       {
//         method: "POST",
//         headers: {
//           Authorization: `Bearer ${accessToken}`,
//         },
//       },
//     );
//     const newRoute = (await res.json()) as IRoute;
//     if (res.ok) {
//       toast.success("Route cloned");
//       dispatch({ type: "ADD_ROUTE", payload: newRoute });
//     }
//   };
//   const handleDeleteClick = async () => {
//     if (confirm("Are you sure want to delete this route?")) {
//       const accessToken = await getAccessTokenSilently();
//       const res = await fetch(
//         `/api/apps/${app.id}/routes/${actionMenuRoute!.id}`,
//         {
//           method: "DELETE",
//           headers: {
//             Authorization: `Bearer ${accessToken}`,
//           },
//         },
//       );
//       if (res.ok) {
//         dispatch({
//           type: "DELETE_ROUTE",
//           payload: routes.find((r) => r.id === actionMenuRoute!.id)!,
//         });
//         if (actionMenuRoute!.id === activeRoute?.id) {
//           navigate("./");
//         }
//       }
//     }
//   };
//   return (
//     <div>
//       <div ref={actionMenuRef} className="relative">
//         {actionMenuRoute && (
//           <ActionMenu
//             top={calculateActionMenuTop()}
//             right={calculateActionMenuRight()}
//             node={actionMenuRoute}
//             onDuplicateClick={handleDuplicateClick}
//             onDeleteClick={handleDeleteClick}
//             onOutSideClick={() => setActionMenuRoute(undefined)}
//           />
//         )}
//       </div>
//       <ul className="mt-1">
//         {routes.map((route) => {
//           return (
//             <li
//               key={route.id}
//               onClick={() => {
//                 navigate(route.id!.toString());
//               }}
//               className={
//                 route.id == activeRoute?.id
//                   ? "bg-slate-100"
//                   : "hover:bg-slate-50"
//               }
//             >
//               <RouteItem
//                 route={route}
//                 onActionMenuClick={(buttonLeft, buttonTop) =>
//                   handleActionMenuClick(route, buttonLeft, buttonTop)
//                 }
//               />
//             </li>
//           );
//         })}
//       </ul>
//     </div>
//   );
// }

function RouteItem({
  route,
  id,
  onActionMenuClick,
}: {
  route: Node["route"];
  id: number;
  onActionMenuClick(buttonLeft: number, buttonTop: number): void;
}) {
  if (!route) {
    return null;
  }

  const navigate = useNavigate();

  const methodTextColors = new Map<string, string>([
    ["GET", "text-sky-500"],
    ["POST", "text-orange-500"],
    ["PUT", "text-green-500"],
    ["DELETE", "text-red-500"],
    ["PATCH", "text-yellow-500"],
  ]);

  const handleClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.stopPropagation();
    onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
      onClick={() => {
        navigate(id.toString());
      }}
    >
      <span
        className={`me-1 font-semibold ${methodTextColors.get(
          route.method.toUpperCase(),
        )}`}
        style={{ fontSize: "0.65rem" }}
      >
        {route.method}
      </span>
      <span className="overflow-hidden text-ellipsis whitespace-nowrap text-sm">
        {route.name}
      </span>
      <button
        type="button"
        onClick={(e) => handleClick(e)}
        className="ms-auto hidden group-hover:block"
      >
        <EllipsisVerticalIcon className="h-4 w-4 text-gray-600" />
      </button>
      {route.status === "Blocked" && (
        <small className="ms-auto text-sm text-red-600">Blocked</small>
      )}
    </div>
  );
}

function FolderItem({
  folder,
  onActionMenuClick,
}: {
  folder: Node["folder"];
  onActionMenuClick(buttonLeft: number, buttonTop: number): void;
}) {
  if (!folder) {
    return null;
  }

  const handleClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.stopPropagation();
    onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
    >
      <span className="overflow-hidden text-ellipsis whitespace-nowrap text-sm">
        {folder.name}
      </span>
      <button
        type="button"
        onClick={(e) => handleClick(e)}
        className="ms-auto hidden group-hover:block"
      >
        <EllipsisVerticalIcon className="h-4 w-4 text-gray-600" />
      </button>
    </div>
  );
}

function ActionMenu({
  node,
  top,
  right,
  onNewRouteClick,
  onNewFolderClick,
  onDuplicateClick,
  onOutSideClick,
  onDeleteClick,
}: {
  node: Node;
  top: number;
  right: number;
  onNewRouteClick(): void;
  onNewFolderClick(): void;
  onDuplicateClick(): void;
  onDeleteClick(): void;
  onOutSideClick(): void;
}) {
  const ref = useRef<HTMLDivElement>(null);
  const windowClickHandler = () => {
    onOutSideClick();
  };
  useEffect(() => {
    window.addEventListener("click", windowClickHandler);

    return () => {
      window.removeEventListener("click", windowClickHandler);
    };
  }, []);
  return (
    <>
      <div
        style={{ top: `${top}px`, right: `${right}px` }}
        ref={ref}
        className="absolute w-[80%] border bg-white shadow"
      >
        <ul>
          {node.folder && (
            <>
              <li className="p-1.5">
                <button
                  type="button"
                  onClick={onNewRouteClick}
                  className="w-full text-left"
                >
                  New Route
                </button>
              </li>
              <li className="p-1.5">
                <button
                  type="button"
                  onClick={onNewFolderClick}
                  className="w-full text-left"
                >
                  New Folder
                </button>
              </li>
            </>
          )}
          <li className="p-1.5">
            <button
              type="button"
              onClick={onDuplicateClick}
              className="w-full text-left"
            >
              Duplicate
            </button>
          </li>
          <hr />
          <li className="p-1.5">
            <button
              type="button"
              className="w-full text-left text-red-500"
              onClick={onDeleteClick}
            >
              Delete
            </button>
          </li>
        </ul>
      </div>
    </>
  );
}
