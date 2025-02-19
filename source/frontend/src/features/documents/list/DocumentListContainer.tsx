import { DocumentRelationshipType } from 'constants/documentRelationshipType';
import { SideBarContext } from 'features/properties/map/context/sidebarContext';
import useIsMounted from 'hooks/useIsMounted';
import { Api_DocumentRelationship } from 'models/api/Document';
import { useCallback, useContext, useEffect, useState } from 'react';
import { toast } from 'react-toastify';

import { useDocumentRelationshipProvider } from '../hooks/useDocumentRelationshipProvider';
import DocumentListView from './DocumentListView';

interface IDocumentListContainerProps {
  parentId: number;
  relationshipType: DocumentRelationshipType;
  disableAdd?: boolean;
  addButtonText?: string;
}

const DocumentListContainer: React.FunctionComponent<
  React.PropsWithChildren<IDocumentListContainerProps>
> = props => {
  const isMounted = useIsMounted();

  const [documentResults, setDocumentResults] = useState<Api_DocumentRelationship[]>([]);

  const { file, staleFile, setStaleFile } = useContext(SideBarContext);

  const {
    retrieveDocumentRelationship,
    retrieveDocumentRelationshipLoading,
    deleteDocumentRelationship,
  } = useDocumentRelationshipProvider();

  const retrieveDocuments = useCallback(async () => {
    const documents = await retrieveDocumentRelationship(props.relationshipType, props.parentId);
    if (documents !== undefined && isMounted()) {
      setDocumentResults(documents);
    }
  }, [isMounted, retrieveDocumentRelationship, props.relationshipType, props.parentId]);

  useEffect(() => {
    retrieveDocuments();
  }, [retrieveDocuments]);

  useEffect(() => {
    if (staleFile) {
      retrieveDocuments();
    }
  }, [staleFile, retrieveDocuments]);

  const onDelete = async (
    documentRelationship: Api_DocumentRelationship,
  ): Promise<boolean | undefined> => {
    if (documentRelationship.relationshipType !== undefined) {
      let result = await deleteDocumentRelationship(
        documentRelationship.relationshipType,
        documentRelationship,
      );
      if (result && isMounted()) {
        updateCallback();
      }

      return result;
    } else {
      toast.error(
        'Invalid document relationship error during delete. Check responses and try again.',
      );
    }
  };

  const onSuccess = async () => {
    updateCallback();
  };

  const updateCallback = useCallback(() => {
    // Check if the component is working in a File context. If it is, delegate the update.
    if (file === undefined) {
      retrieveDocuments();
    } else {
      setStaleFile(true);
    }
  }, [file, setStaleFile, retrieveDocuments]);

  return (
    <DocumentListView
      parentId={props.parentId}
      relationshipType={props.relationshipType}
      addButtonText={props.addButtonText}
      isLoading={retrieveDocumentRelationshipLoading}
      documentResults={documentResults}
      onDelete={onDelete}
      onSuccess={onSuccess}
      disableAdd={props.disableAdd}
    />
  );
};

export default DocumentListContainer;
