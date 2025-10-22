export interface Member {
  id: string;
  birthDay: string;
  imageUrl?: string;
  displayName: string;
  created: string;
  lastActive: string;
  gender: string;
  descripction?: string;
  city: string;
  country: string;
}

export interface Photo {
  id: number;
  url: string;
  publicId?: string;
  memberId: string;
}
