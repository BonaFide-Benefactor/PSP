import { DocumentRelationshipType } from 'constants/documentRelationshipType';
import {
  Api_DocumentRelationship,
  Api_DocumentType,
  Api_DocumentUpdateRequest,
  Api_DocumentUpdateResponse,
  Api_DocumentUploadRequest,
  Api_DocumentUploadResponse,
} from 'models/api/Document';
import {
  Api_Storage_DocumentMetadata,
  Api_Storage_DocumentTypeMetadataType,
  DocumentQueryResult,
  FileDownload,
} from 'models/api/DocumentStorage';
import { ExternalResult } from 'models/api/ExternalResult';
import React from 'react';

import { useAxiosApi } from '.';

/**
 * PIMS API wrapper to centralize all AJAX requests to the note endpoints.
 * @returns Object containing functions to make requests to the PIMS API.
 */

export const useApiDocuments = () => {
  const api = useAxiosApi();

  return React.useMemo(
    () => ({
      getDocumentTypes: () => api.get<Api_DocumentType[]>(`/documents/types`),

      getDocumentTypeMetadata: (mayanDocumentTypeId: number) =>
        api.get<ExternalResult<DocumentQueryResult<Api_Storage_DocumentTypeMetadataType>>>(
          `/documents/storage/types/${mayanDocumentTypeId}/metadata`,
        ),

      getDocumentRelationship: (relationshipType: DocumentRelationshipType, parentId: number) =>
        api.get<Api_DocumentRelationship[]>(`/documents/${relationshipType}/${parentId}`),

      deleteDocumentRelationshipApiCall: (
        relationshipType: DocumentRelationshipType,
        documentRelationship: Api_DocumentRelationship,
      ) => api.delete<boolean>(`/documents/${relationshipType}`, { data: documentRelationship }),

      getDocumentMetada: (mayanDocumentId: number) =>
        api.get<ExternalResult<DocumentQueryResult<Api_Storage_DocumentMetadata>>>(
          `/documents/storage/${mayanDocumentId}/metadata`,
        ),

      downloadDocumentFileApiCall: (mayanDocumentId: number, mayanFileId: number) =>
        api.get<ExternalResult<FileDownload>>(
          `/documents/storage/${mayanDocumentId}/files/${mayanFileId}/download`,
        ),

      downloadDocumentFileLatestApiCall: (mayanDocumentId: number) =>
        api.get<ExternalResult<FileDownload>>(`/documents/storage/${mayanDocumentId}/download`),

      uploadDocumentRelationshipApiCall: (
        relationshipType: DocumentRelationshipType,
        parentId: number,
        uploadRequest: Api_DocumentUploadRequest,
      ) => {
        const formData = new FormData();
        formData.append('file', uploadRequest.file);
        formData.append(
          'documentTypeMayanId',
          uploadRequest.documentType.mayanId?.toString() || '',
        );
        formData.append('documentTypeId', uploadRequest.documentType.id?.toString() || '');
        formData.append('documentStatusCode', uploadRequest.documentStatusCode || '');

        uploadRequest.documentMetadata?.forEach((metadata, index) => {
          formData.append(
            'DocumentMetadata[' + index + '].MetadataTypeId',
            metadata.metadataTypeId.toString(),
          );
          formData.append('DocumentMetadata[' + index + '].Value', metadata.value);
          index++;
        });

        return api.post<Api_DocumentUploadResponse>(
          `/documents/upload/${relationshipType}/${parentId}`,
          formData,
        );
      },
      updateDocumentMetadataApiCall: (
        documentId: number,
        updateRequest: Api_DocumentUpdateRequest,
      ) => {
        return api.put<Api_DocumentUpdateResponse>(
          `/documents/${documentId}/metadata`,
          updateRequest,
        );
      },
    }),
    [api],
  );
};
