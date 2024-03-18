export default interface Item {
  type: "file" | "folder";
  name: string;
  id: number;
  size: number;
  createdAt: string;
}
