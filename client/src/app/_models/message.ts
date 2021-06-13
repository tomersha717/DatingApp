export interface Message {
  id: number
  senderId: any
  senderUsername: string
  senderPhotoUrl: string
  recipientId: number
  recipientUsername: string
  recipietnPhotoUrl: string
  content: string
  dateRead?: Date
  messageSent: Date
}
