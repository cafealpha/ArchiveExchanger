   M S B A M L   `   `   `  ����K  GarchiveExchanger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:archiveExchangerarchiveExchanger  5   	   !    archiveExchanger.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:archiveExchanger     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   $	��800��5	   	   $	��450��5
   	    � DataContext 5   	   ��  �� RelativeSource �� *Self+.��5      �  � RowDefinitions 5   
   �� 5      $	��150 �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ btnAddFolder��5      $��0��6   $��폴더 추가��64   $
��Left=�6D   $��	10,10,0,0q�6_   $	��Top=�6r   $��75��6�   6�   �� 5   
   -   5        $ 
btnAddFile��5      $��0��6   $��파일 추가��62   $
��Left=�6B   $��	10,40,0,0q�6]   $	��Top=�6p   $��75��6�   6�   �� 5   
   $��0��6   $��변환할 포멧��6!   $
��Left=�6/   $��70��6K   $��20��6V   $��293,17,0,113q�6b   �� 5   
   -   5         ��Name$ cbDestFormat��5       $��0��6   $
��Left=�67   $��
368,14,0,0q�6R   $	��Top=�6f   $	��120��6~    �� SelectedIndex$ 0��6�   .@�5      �� $	��ZIP��6   �� 5      $��7z��6   5      �� 5   
   -   5        $ btnConvertStart��5      $��0��6   $��변환시작��67   $��Right=�6F   $��	0,0,10,10q�6b   $��Bottom=�6u   $��75��6�   6�   �� 5   
   $��검색할 압축포멧��6   $
��Left=�6%   $	��100��6@   $��	90,10,0,0q�6M   $	��Top=�6`   $	��195��6x   .��5      �� -   5         ��Name$ sbCheck��5      $��5q�6*   .+�5      �� -   5         ��Name$ cbRAR��5      $	��RAR��6*   	 d� 	IsChecked$
	 trueY�68   �� 5      -   5        $
 cb7z��5      $��7z��6)   �� 5      -   5        $ cbZIP��5      $	��ZIP��6*   5       5!      �� 5"   
   -	   5        
 ��Name$
 
tbDebugBox��5"      $��0��6   $
��Left=�63   $��79��6N   $��
506,17,0,0q�6Z    �� TextWrapping$
 Wrap=�6n   $�� ��6�   $	��Top=�6�   $	��276��6�   �� 5$   
   -
   5         ��Name$ 
lvFileList��5$       $��1��6   ��64   ��  �� Path$ Items� �� View 5%      �� 5&      .P�5'      �� $��소스파일이름��6%    �� DisplayMemberBinding 66   �� *sourceFileName+�� 5(      $��변환될 파일이름��6%    68   �� *destFileName+�� 5)      $��경로��6%    62   �� *origin+�� 5*      $��변환할 포멧��6%    �� CellTemplate 5+      �� 5,      .R�5-   "   � .+�5.   &   �� $��50��50   /   ��5.   /   �� *
destFormat+ �� SelectedItem 5/   /   �� *	destExt+50   /   51   #   52      54      �� 55      $��	진행율��6%    56      �� 57      .R�58   "   � .+�59   &   �  �Name$ pbAll��62   -   5        $��20��59   A   $	��150��6M    � Value 6Y   �� *
progress+6t   �� 5:   &   $��Center=�6_   $��Center=�6|   ��6,   �� $ Value� �� ElementName$ pbAll��6|   5;   #   5<      5>      5?      5A      5D      5F      