import { ChildComponent } from "./ChildComponent";
import { MonacoEditor } from "./MonacoEditor";
import { MonacoEditor2 } from "./MonacoEditor2";

const routes = [
  {
    path: "MonacoEditor",
    name: "MonacoEditor",
    component: MonacoEditor,
  },
  {
    path: "MonacoEditor2",
    name: "MonacoEditor2",
    component: MonacoEditor2
  },
  {
    path: "ChildComponent",
    name: "ChildComponent",
    component: ChildComponent,
  },
];

export default routes;
