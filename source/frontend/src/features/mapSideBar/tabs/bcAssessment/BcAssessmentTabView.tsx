import { FormSection } from 'components/common/form/styles';
import LoadingBackdrop from 'components/maps/leaflet/LoadingBackdrop/LoadingBackdrop';
import { IBcAssessmentSummary } from 'hooks/useBcAssessmentLayer';
import moment from 'moment';
import * as React from 'react';
import styled from 'styled-components';
import { pidFormatter } from 'utils/propertyUtils';

import { Section } from '../Section';
import { SectionField } from '../SectionField';
import { InlineMessage, StyledInlineMessageSection } from '../SectionStyles';
import AssessedValuesTable from './AssessedValuesTable';
import SalesTable from './SalesTable';

export interface IBcAssessmentTabViewProps {
  summaryData?: IBcAssessmentSummary;
  requestedOn?: Date;
  loading: boolean;
  pid?: string;
}

export const BcAssessmentTabView: React.FunctionComponent<IBcAssessmentTabViewProps> = ({
  summaryData,
  requestedOn,
  loading,
  pid,
}) => {
  const address = summaryData?.ADDRESSES?.find(a => a.PRIMARY_IND === 'true');

  return (
    <>
      <LoadingBackdrop show={loading} parentScreen={true} />

      {!pid ? (
        <FormSection>
          <b>
            This property does not have a valid PID.
            <br />
            <br /> Only properties that are associated to a valid PID can display corresponding data
            from LTSA.
          </b>
        </FormSection>
      ) : !loading && !summaryData ? (
        <FormSection>
          <b>
            Failed to load data from BC Assessment.
            <br />
            <br /> Refresh this page to try again, or select a different property.
          </b>
        </FormSection>
      ) : (
        <StyledForm>
          {requestedOn && (
            <StyledInlineMessageSection>
              <InlineMessage>
                This data was retrieved from BC Assessment on{' '}
                {moment(requestedOn).format('DD-MMM-YYYY h:mm A')}
              </InlineMessage>
            </StyledInlineMessageSection>
          )}
          <Section header="Assessment Overview">
            <SectionField label="PID">{pidFormatter(pid)}</SectionField>
            <SectionField label="Jurisdiction">
              {summaryData?.FOLIO_DESCRIPTION?.JURISDICTION_CODE
                ? `${summaryData?.FOLIO_DESCRIPTION.JURISDICTION_CODE} - ${summaryData?.FOLIO_DESCRIPTION.JURISDICTION}`
                : ''}
            </SectionField>
            <SectionField label="Neighbourhood">
              {summaryData?.FOLIO_DESCRIPTION?.NEIGHBOURHOOD_CODE
                ? `${summaryData?.FOLIO_DESCRIPTION.NEIGHBOURHOOD_CODE} - ${summaryData?.FOLIO_DESCRIPTION.NEIGHBOURHOOD}`
                : ''}
            </SectionField>
            <SectionField label="Ownership year">Not available</SectionField>
            <SectionField label="Roll number">
              {summaryData?.FOLIO_DESCRIPTION?.ROLL_NUMBER ?? ''}
            </SectionField>
            <SectionField label="Roll year">Not available</SectionField>
            <SectionField label="Document number">Not available</SectionField>
          </Section>
          <Section header="Property Address">
            <StyledSubtleText>
              This is the property address as per BC Assessment (for reference).
            </StyledSubtleText>

            {address !== undefined ? (
              <SectionField label="Address">
                {[
                  address?.UNIT_NUMBER,
                  address?.STREET_NUMBER,
                  address?.STREET_DIRECTION_PREFIX,
                  address?.STREET_NAME,
                  address?.STREET_TYPE,
                  address?.STREET_DIRECTION_SUFFIX,
                ]
                  .filter(a => !!a)
                  .join(' ')}
              </SectionField>
            ) : (
              <b>Unable to determine address from BC Assessment</b>
            )}
            <SectionField label="City">{address?.CITY ?? ''}</SectionField>
            <SectionField label="Province">{address?.PROVINCE ?? ''}</SectionField>
            <SectionField label="Postal code">{address?.POSTAL_CODE ?? ''}</SectionField>
          </Section>
          <Section header="Assessed Value">
            <AssessedValuesTable valuesData={summaryData?.VALUES ?? []} />
          </Section>
          <Section header="Assessment Details">
            <SectionField label="Manual class">
              {summaryData?.FOLIO_DESCRIPTION?.MANUAL_CLASS_CODE
                ? `${summaryData?.FOLIO_DESCRIPTION.MANUAL_CLASS_CODE} - ${summaryData?.FOLIO_DESCRIPTION.MANUAL_CLASS_DESCRIPTION}`
                : ''}
            </SectionField>
            <SectionField label="Actual use">
              {summaryData?.FOLIO_DESCRIPTION?.ACTUAL_USE_CODE
                ? `${summaryData?.FOLIO_DESCRIPTION.ACTUAL_USE_CODE} - ${summaryData?.FOLIO_DESCRIPTION.ACTUAL_USE_DESCRIPTION}`
                : ''}
            </SectionField>
            <SectionField label="ALR">
              {summaryData?.FOLIO_DESCRIPTION?.ALR_CODE
                ? `${summaryData?.FOLIO_DESCRIPTION.ALR_CODE} - ${summaryData?.FOLIO_DESCRIPTION.ALR_DESCRIPTION}`
                : ''}
            </SectionField>
            <SectionField label="Land dimension">
              {`${summaryData?.FOLIO_DESCRIPTION?.LAND_SIZE ?? ''} ${
                summaryData?.FOLIO_DESCRIPTION?.LAND_UNITS ?? ''
              } `}
            </SectionField>
          </Section>
          <Section header="Sales">
            <SalesTable salesData={summaryData?.SALES} />
          </Section>
        </StyledForm>
      )}
    </>
  );
};

export const StyledForm = styled.div`
  position: relative;
  &&& {
    input,
    select,
    textarea {
      background: none;
      border: none;
      resize: none;
      height: fit-content;
      padding: 0;
    }
    .form-label {
      font-weight: bold;
    }
  }
`;

const StyledSubtleText = styled.p`
  color: ${props => props.theme.css.subtleColor};
  text-align: left;
`;

export default BcAssessmentTabView;
