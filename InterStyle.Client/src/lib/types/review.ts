export interface Review {
	id: string;
	customerName: string;
	rating: number;
	comment: string;
	isApproved: boolean;
	createdAtUtc: string;
	approvedAtUtc: string | null;
}

export interface ReviewPage {
	items: Review[];
	page: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
}

export interface SubmitReviewRequest {
	customerName: string;
	rating: number;
	comment: string;
}
