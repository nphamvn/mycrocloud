export default interface IConversation {
  id: number;
  name?: string;
  lastMessage: string;
  members: IUser[];
}
